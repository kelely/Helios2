using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Helios.Caching;
using Helios.Common.Domain;
using Helios.Data;
using Rhino.Mocks;
using Xunit;

namespace Helios.Common.Services
{
    public class GenericAttributeServiceTests 
    {
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;
        private readonly IGenericAttributeService _genericAttributeService;

        public GenericAttributeServiceTests()
        {
            var attributes = new List<GenericAttribute>
            {
                new GenericAttribute { Id = 1, EntityId = 1, EntityGroup = "helios.domain.customers.customer", Key = "FirstName", Value = "Joy" },
                new GenericAttribute { Id = 2, EntityId = 1, EntityGroup = "helios.domain.customers.customer", Key = "LastName", Value = "Steve" },
                new GenericAttribute { Id = 3, EntityId = 1, EntityGroup = "helios.domain.customers.customer", Key = "Gender", Value = "True" },
                new GenericAttribute { Id = 4, EntityId = 1, EntityGroup = "helios.domain.customers.customer", Key = "DateOfBirth", Value = "1999-04-21" },

                new GenericAttribute { Id = 6, EntityId = 2, EntityGroup = "helios.domain.customers.customer", Key = "FirstName", Value = "Yang" },
                new GenericAttribute { Id = 7, EntityId = 2, EntityGroup = "helios.domain.customers.customer", Key = "LastName", Value = "Qian" },
                new GenericAttribute { Id = 8, EntityId = 2, EntityGroup = "helios.domain.customers.customer", Key = "Gender", Value = "False" },
                new GenericAttribute { Id = 9, EntityId = 2, EntityGroup = "helios.domain.customers.customer", Key = "DateOfBirth", Value = "1996-09-16" },
            };
            _genericAttributeRepository = MockRepository.GenerateMock<IRepository<GenericAttribute>>();
            _genericAttributeRepository.Stub(x => x.Table).Return(attributes.AsQueryable());
            _genericAttributeRepository.Stub(x => x.TableNoTracking).Return(_genericAttributeRepository.Table);

            //_eventPublisher = MockRepository.GenerateMock<IEventPublisher>();

            _genericAttributeService = new GenericAttributeService(new HeliosNullCache(), _genericAttributeRepository);
        }

        [Fact(DisplayName = "确保可以通过Id获取通用属性实体对象")]
        public void Can_get_attribute_by_id()
        {
            var ga = _genericAttributeService.GetAttributeById(1);

            ga.Should().NotBeNull();
            ga.Id.Should().Be(1);
            ga.EntityId.Should().Be(1);
        }

        [Fact(DisplayName = "确保可以通过实体Id和名称获取实体扩展属性集合")]
        public void Can_get_attributes_for_entity()
        {
            var attributes = _genericAttributeService.GetAttributesForEntity(1, "helios.domain.customers.customer");
            attributes.Should().NotBeNull().And.HaveCount(4);

            // 验证大小写无关
            attributes = _genericAttributeService.GetAttributesForEntity(1, "Helios.Domain.Customers.Customer");
            attributes.Should().NotBeNull().And.HaveCount(4);
        }

        [Fact(DisplayName = "确保在扩展属性值为空字符串时，会将通用属性数据删除")]
        public void Can_delete_generic_attribute_with_empty_value()
        {
            var ga = new GenericAttribute { Id = 4, EntityId = 1, EntityGroup = "helios.domain.customers.customer", Key = "DateOfBirth", Value = "" };
            _genericAttributeService.SaveAttribute(ga);

            _genericAttributeRepository.AssertWasCalled(x => x.Delete(ga));
            //_eventPublisher.AssertWasCalled(x => x.Publish(Arg<EntityDeleted<GenericAttribute>>.Is.Anything));
        }

        [Fact(DisplayName = "确保在扩展属性值为空对象时，会将通用属性数据删除")]
        public void Can_delete_generic_attribute_with_null_object()
        {
            var ga = new GenericAttribute { Id = 4, EntityId = 1, EntityGroup = "helios.domain.customers.customer", Key = "DateOfBirth", Value = null };
            _genericAttributeService.SaveAttribute(ga);

            _genericAttributeRepository.AssertWasCalled(x => x.Delete(ga));
            //_eventPublisher.AssertWasCalled(x => x.Publish(Arg<EntityDeleted<GenericAttribute>>.Is.Anything));
        }

        [Fact(DisplayName = "确保可以增加通用属性实体")]
        public void Can_insert_generic_attribute()
        {
            var ga = new GenericAttribute { Id = 0, EntityId = 1, EntityGroup = "helios.domain.customers.customer", Key = "DateOfBirth", Value = "2016-09-16" };
            _genericAttributeService.SaveAttribute(ga);

            _genericAttributeRepository.AssertWasCalled(x => x.Insert(ga));
            //_eventPublisher.AssertWasCalled(x => x.Publish(Arg<EntityInserted<GenericAttribute>>.Is.Anything));
        }

        [Fact(DisplayName = "确保可以更新通用属性实体对象")]
        public void Can_update_generic_attribute()
        {
            var ga = new GenericAttribute { Id = 7, EntityId = 2, EntityGroup = "helios.domain.customers.customer", Key = "LastName", Value = "Guang" };
            _genericAttributeService.SaveAttribute(ga);

            _genericAttributeRepository.AssertWasCalled(x => x.Update(ga));
            //_eventPublisher.AssertWasCalled(x => x.Publish(Arg<EntityUpdated<GenericAttribute>>.Is.Anything));
        }
    }
}
