using BudgetManager.Data.DataModels;
using BudgetManager.ManagerWeb.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.ManagerWeb.Services
{
    internal class TagService : ITagService
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
            PaymentTag paymentTag = new PaymentTag
            {
                PaymentId = tagModel.PaymentId
            };

            Tag tag = this.tagRepository.FindAll().ToList().SingleOrDefault(t => string.Compare(t.Code, tagModel.Code, true) == 0);

            if (tag is null)
            {
                tag = new Tag
                {
                    Code = tagModel.Code,
                };
                this.tagRepository.Create(tag);
                paymentTag.Tag = tag;
            }
            else
            {
                paymentTag.TagId = tag.Id;
            }

            this.paymentTagRepository.Create(paymentTag);
            this.paymentTagRepository.Save();
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

        public void UpdateAllTags(List<string> tags, int paymentId)
        {
            List<(string tag, int tagId)> tagsOnPayment = this.paymentTagRepository.FindByCondition(t => t.PaymentId == paymentId).Include(i => i.Tag).ToList().Select(m => (tag: m.Tag.Code, tagId: m.TagId)).ToList();

            IEnumerable<(string tag, int tagId)> toDelete = tagsOnPayment.Where(t => tags.Contains(t.tag));
            IEnumerable<string> toAdd = tags.Where(t => !tagsOnPayment.Exists(a => a.tag == t));

            foreach ((string tag, int tagId) in toDelete)
            {
                this.RemoveTagFromPayment(tagId, paymentId);
            }

            foreach (string tag in toAdd)
            {
                this.AddTagToPayment(new AddTagModel { Code = tag, PaymentId = paymentId });
            }
        }
    }
}
