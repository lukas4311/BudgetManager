﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BudgetManager.Services
{
    /// <inheritdoc/>
    internal class TagService : BaseService<TagModel, Tag, IRepository<Tag>>, ITagService
    {
        private const string DoesntExists = "Tag doesn't exists";

        /// <summary>
        /// The repository for managing tags.
        /// </summary>
        private readonly IRepository<Tag> tagRepository;

        /// <summary>
        /// The repository for managing payment tags.
        /// </summary>
        private readonly IRepository<PaymentTag> paymentTagRepository;

        /// <summary>
        /// The repository for managing user identities.
        /// </summary>
        private readonly IRepository<UserIdentity> userIdentityRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagService"/> class.
        /// </summary>
        /// <param name="tagRepository">The tag repository.</param>
        /// <param name="paymentTagRepository">The payment tag repository.</param>
        /// <param name="userIdentityRepository">The user identity repository.</param>
        /// <param name="autoMapper">The auto mapper.</param>
        public TagService(IRepository<Tag> tagRepository, IRepository<PaymentTag> paymentTagRepository, IRepository<UserIdentity> userIdentityRepository, IMapper autoMapper)
            : base(tagRepository, autoMapper)
        {
            this.tagRepository = tagRepository;
            this.paymentTagRepository = paymentTagRepository;
            this.userIdentityRepository = userIdentityRepository;
        }

        /// <inheritdoc/>
        public IEnumerable<TagModel> GetPaymentTags()
        {
            return userIdentityRepository.FindAll()
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

        /// <inheritdoc/>
        public IEnumerable<TagModel> GetPaymentsTags(int userId)
        {
            return userIdentityRepository.FindByCondition(u => u.Id == userId)
                .Include(p => p.BankAccounts)
                .SelectMany(a => a.BankAccounts)
                .SelectMany(t => t.Payments)
                .SelectMany(p => p.PaymentTags)
                .Select(t => new TagModel
                {
                    Code = t.Tag.Code,
                    Id = t.Tag.Id
                })
                .Distinct();
        }

        /// <inheritdoc/>
        public void AddTagToPayment(AddTagModel tagModel)
        {
            PaymentTag paymentTag = new PaymentTag
            {
                PaymentId = tagModel.PaymentId
            };

            Tag tag = tagRepository.FindAll().ToList().SingleOrDefault(t => string.Compare(t.Code, tagModel.Code, true) == 0);

            if (tag is null)
            {
                tag = new Tag
                {
                    Code = tagModel.Code,
                };
                tagRepository.Create(tag);
                paymentTag.Tag = tag;
            }
            else
            {
                paymentTag.TagId = tag.Id;
            }

            paymentTagRepository.Create(paymentTag);
            paymentTagRepository.Save();
        }

        /// <inheritdoc/>
        public void RemoveTagFromPayment(int tagId, int paymentId)
        {
            PaymentTag paymentTag = paymentTagRepository.FindByCondition(t => t.PaymentId == paymentId && t.TagId == tagId).Single();

            paymentTagRepository.Delete(paymentTag);
            paymentTagRepository.Save();
        }

        /// <summary>
        /// Deletes a tag.
        /// </summary>
        /// <param name="tagId">The tag ID.</param>
        public override void Delete(int tagId)
        {
            Tag tag = tagRepository.FindByCondition(t => t.Id == tagId).SingleOrDefault();

            if (tag is null)
                throw new ArgumentException(DoesntExists);

            foreach (PaymentTag paymentTag in paymentTagRepository.FindByCondition(a => a.TagId == tag.Id))
                paymentTagRepository.Delete(paymentTag);

            tagRepository.Delete(tag);
            tagRepository.Save();
        }

        /// <inheritdoc/>
        public void UpdateAllTags(List<string> tags, int paymentId)
        {
            List<(string tag, int tagId)> tagsOnPayment = paymentTagRepository.FindByCondition(t => t.PaymentId == paymentId).Include(i => i.Tag).ToList().Select(m => (tag: m.Tag.Code, tagId: m.TagId)).ToList();

            IEnumerable<(string tag, int tagId)> toDelete = tagsOnPayment.Where(t => tags.Contains(t.tag));
            IEnumerable<string> toAdd = tags.Where(t => !tagsOnPayment.Exists(a => a.tag == t));

            foreach ((string tag, int tagId) in toDelete)
                RemoveTagFromPayment(tagId, paymentId);

            foreach (string tag in toAdd)
                AddTagToPayment(new AddTagModel { Code = tag, PaymentId = paymentId });
        }
    }
}
