using System;
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
    /// <summary>
    /// Represents a service for managing tags.
    /// </summary>
    internal class TagService : BaseService<TagModel, Tag, ITagRepository>, ITagService
    {
        private const string DoesntExists = "Tag doesn't exists";

        /// <summary>
        /// The repository for managing tags.
        /// </summary>
        private readonly ITagRepository tagRepository;

        /// <summary>
        /// The repository for managing payment tags.
        /// </summary>
        private readonly IPaymentTagRepository paymentTagRepository;

        /// <summary>
        /// The repository for managing user identities.
        /// </summary>
        private readonly IUserIdentityRepository userIdentityRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagService"/> class.
        /// </summary>
        /// <param name="tagRepository">The tag repository.</param>
        /// <param name="paymentTagRepository">The payment tag repository.</param>
        /// <param name="userIdentityRepository">The user identity repository.</param>
        /// <param name="autoMapper">The auto mapper.</param>
        public TagService(ITagRepository tagRepository, IPaymentTagRepository paymentTagRepository, IUserIdentityRepository userIdentityRepository, IMapper autoMapper)
            : base(tagRepository, autoMapper)
        {
            this.tagRepository = tagRepository;
            this.paymentTagRepository = paymentTagRepository;
            this.userIdentityRepository = userIdentityRepository;
        }

        /// <summary>
        /// Gets the payment tags associated with user identities.
        /// </summary>
        /// <returns>An enumerable of <see cref="TagModel"/>.</returns>
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

        /// <summary>
        /// Gets the payment tags associated with a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>An enumerable of <see cref="TagModel"/>.</returns>
        public IEnumerable<TagModel> GetPaymentsTags(int userId)
        {
            return this.userIdentityRepository.FindByCondition(u => u.Id == userId)
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

        /// <summary>
        /// Adds a tag to a payment.
        /// </summary>
        /// <param name="tagModel">The tag model containing payment information.</param>
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

        /// <summary>
        /// Removes a tag from a payment.
        /// </summary>
        /// <param name="tagId">The tag ID.</param>
        /// <param name="paymentId">The payment ID.</param>
        public void RemoveTagFromPayment(int tagId, int paymentId)
        {
            PaymentTag paymentTag = this.paymentTagRepository.FindByCondition(t => t.PaymentId == paymentId && t.TagId == tagId).Single();

            this.paymentTagRepository.Delete(paymentTag);
            this.paymentTagRepository.Save();
        }

        /// <summary>
        /// Deletes a tag.
        /// </summary>
        /// <param name="tagId">The tag ID.</param>
        public override void Delete(int tagId)
        {
            Tag tag = this.tagRepository.FindByCondition(t => t.Id == tagId).SingleOrDefault();

            if (tag is null)
                throw new ArgumentException(DoesntExists);

            foreach (PaymentTag paymentTag in this.paymentTagRepository.FindByCondition(a => a.TagId == tag.Id))
                this.paymentTagRepository.Delete(paymentTag);

            this.tagRepository.Delete(tag);
            this.tagRepository.Save();
        }

        /// <summary>
        /// Updates tags for a payment.
        /// </summary>
        /// <param name="tags">The list of tags to update.</param>
        /// <param name="paymentId">The payment ID.</param>
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
