using System;
using System.Collections.Generic;
using System.Linq;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BudgetManager.Services
{
    internal class TagService : ITagService
    {
        private const string DoesntExists = "Tag doesn't exists";
        private readonly ITagRepository tagRepository;
        private readonly IPaymentTagRepository paymentTagRepository;
        private readonly IUserIdentityRepository userIdentityRepository;

        public TagService(ITagRepository tagRepository, IPaymentTagRepository paymentTagRepository, IUserIdentityRepository userIdentityRepository)
        {
            this.tagRepository = tagRepository;
            this.paymentTagRepository = paymentTagRepository;
            this.userIdentityRepository = userIdentityRepository;
        }

        public IEnumerable<TagModel> GetPaymentTags()
        {
            return this.userIdentityRepository.FindAll()
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

        public IEnumerable<TagModel> GetPaymentTags(int userId)
        {
            return this.userIdentityRepository.FindByCondition(u => u.Id == userId)
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

        public void Delete(int tagId)
        {
            Tag tag = this.tagRepository.FindByCondition(t => t.Id == tagId).SingleOrDefault();

            if (tag is null)
                throw new ArgumentException(DoesntExists);

            foreach (PaymentTag paymentTag in this.paymentTagRepository.FindByCondition(a => a.TagId == tag.Id))
                this.paymentTagRepository.Delete(paymentTag);

            this.tagRepository.Delete(tag);
            this.tagRepository.Save();
        }

        public void UpdateAllTags(List<string> tags, int paymentId)
        {
            List<(string tag, int tagId)> tagsOnPayment = this.paymentTagRepository.FindByCondition(t => t.PaymentId == paymentId).Include(i => i.Tag).ToList().Select(m => (tag: m.Tag.Code, tagId: m.TagId)).ToList();

            IEnumerable<(string tag, int tagId)> toDelete = tagsOnPayment.Where(t => tags.Contains(t.tag));
            IEnumerable<string> toAdd = tags.Where(t => !tagsOnPayment.Exists(a => a.tag == t));

            foreach ((string tag, int tagId) in toDelete)
                this.RemoveTagFromPayment(tagId, paymentId);

            foreach (string tag in toAdd)
                this.AddTagToPayment(new AddTagModel { Code = tag, PaymentId = paymentId });
        }
    }
}
