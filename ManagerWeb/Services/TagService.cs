using Data.DataModels;
using ManagerWeb.Models.DTOs;
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

        public TagService(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        public IEnumerable<TagModel> GetPaymentTags()
        {
            return this.tagRepository.FindAll().Select(t => new TagModel
            {
                Code = t.Code,
                Id = t.Id
            });
        }

        public void AddTag(TagModel tagModel)
        {
            bool tagCodeExists = this.tagRepository.FindByCondition(t => string.Compare(t.Code, tagModel.Code, true) == 0).Any();

            if (tagCodeExists)
                throw new ArgumentException(AlreadyExist);
        }

        public void DeleteTag(int tagId)
        {
            Tag tag = this.tagRepository.FindByCondition(t => t.Id == tagId).SingleOrDefault();

            if (tag == null)
                throw new ArgumentException(DoesntExists);

            this.tagRepository.Delete(tag);
        }
    }
}
