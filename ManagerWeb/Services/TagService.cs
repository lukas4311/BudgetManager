using Data.DataModels;
using ManagerWeb.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerWeb.Services
{
    public class TagService : ITagService
    {
        private const string AlreadyExist = "Tag with this code already exists";
        private const string DoesntExists = "Tag doesn't exists";
        private readonly ITagRepository tagRepository;
        private readonly IPaymentTagRepository paymentTagRepository;
        private readonly IUserIdentityRepository userIdentityRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TagService(ITagRepository tagRepository, IPaymentTagRepository paymentTagRepository, IUserIdentityRepository userIdentityRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.tagRepository = tagRepository;
            this.paymentTagRepository = paymentTagRepository;
            this.userIdentityRepository = userIdentityRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<TagModel> GetPaymentTags()
        {
            string userName = this.httpContextAccessor.HttpContext.User.Identity.Name;
            return this.userIdentityRepository.FindByCondition(u => u.Login == userName)
                .Include(p => p.BankAccounts)
                .SelectMany(a => a.BankAccounts)
                .SelectMany(t => t.Payments)
                .SelectMany(p => p.PaymentTags)
                .Select(t => new TagModel
                {
                    Code = t.Tag.Code,
                    Id = t.Id
                })
                .Distinct();
        }

        public void AddTagToPayment(AddTagModel tagModel)
        {
            bool tagCodeExists = this.tagRepository.FindByCondition(t => string.Compare(t.Code, tagModel.Code, true) == 0).Any();

            if (tagCodeExists)
                throw new ArgumentException(AlreadyExist);

            Tag tag = new Tag
            {
                Code = tagModel.Code,
            };

            this.tagRepository.Create(tag);
            this.paymentTagRepository.Create(new PaymentTag
            {
                PaymentId = tagModel.PaymentId,
                TagId = tag.Id
            });

            this.tagRepository.Save();
        }

        public void RemoveTagFromPayment(int tagId, int paymentId)
        {
            PaymentTag paymentTag = this.paymentTagRepository.FindByCondition(t => t.PaymentId == paymentId && t.TagId == tagId).Single();

            this.paymentTagRepository.Delete(paymentTag);
            this.paymentTagRepository.Save();
        }

        public void DeleteTag(int tagId)
        {
            Tag tag = this.tagRepository.FindByCondition(t => t.Id == tagId).SingleOrDefault();

            if (tag == null)
                throw new ArgumentException(DoesntExists);

            foreach (PaymentTag paymentTag in this.paymentTagRepository.FindByCondition(a => a.TagId == tag.Id))
            {
                this.paymentTagRepository.Delete(paymentTag);
            }

            this.tagRepository.Delete(tag);
            this.tagRepository.Save();
        }
    }
}
