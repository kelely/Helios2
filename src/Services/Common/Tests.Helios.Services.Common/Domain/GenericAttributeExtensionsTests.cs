﻿using System.Collections.Generic;
using FluentAssertions;
using Helios.Domain;
using Helios.Domain.Common;
using Helios.Services.Common;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Xunit;

namespace Tests.Helios.Domain
{
    public class GenericAttributeExtensionsTests
    {
        private readonly IGenericAttributeService _genericAttributeService;

        public GenericAttributeExtensionsTests()
        {
            var attributes = new List<GenericAttribute>
            {
                new GenericAttribute { Id = 1, EntityId = 1, EntityGroup = "helios.domain.common.genericattribute", Key = "FirstName", Value = "Joy" },
                new GenericAttribute { Id = 2, EntityId = 1, EntityGroup = "helios.domain.common.genericattribute", Key = "LastName", Value = "Steve" },
                new GenericAttribute { Id = 3, EntityId = 1, EntityGroup = "helios.domain.common.genericattribute", Key = "Gender", Value = "True" },
                new GenericAttribute { Id = 4, EntityId = 1, EntityGroup = "helios.domain.common.genericattribute", Key = "DateOfBirth", Value = "1999-04-21" },
            };

            _genericAttributeService = MockRepository.GenerateMock<IGenericAttributeService>();
            _genericAttributeService.Stub(x => x.GetAttributesForEntity(Arg<int>.Is.Anything, Arg<string>.Is.Anything))
                .Return(attributes);
        }

        [Fact(DisplayName = "确保可以通过实体对象的扩展方法新增扩展属性")]
        public void Ensure_insert_generic_attribute_via_entity_extensions_method()
        {
            var entity = new GenericAttribute { Id = 1 };
            entity.SaveAttribute("Property1", true, _genericAttributeService);

            _genericAttributeService.AssertWasCalled(x => x.SaveAttribute(Arg<GenericAttribute>.Matches(ga => ga.Id == 0 && ga.Value == "True" && ga.Key == "property1")));
        }

        [Fact(DisplayName = "确保可以通过实体对象的扩展方法更新扩展属性")]
        public void Ensure_update_generic_attribute_via_entity_extensions_method()
        {
            var entity = new GenericAttribute { Id = 1 };

            // 将对象已有的属性(FirstName)的属性值进行变更
            entity.SaveAttribute("FirstName", "Guo", _genericAttributeService);

            _genericAttributeService.AssertWasCalled(x => x.SaveAttribute(Arg<GenericAttribute>.Matches(ga=> ga.Id == 1 && ga.Value == "Guo")));
        }

        [Fact(DisplayName = "确保扩展属性值为空字符串时会删除扩展属性")]
        public void Ensure_delete_generic_attribute_via_entity_extensions_method()
        {
            var entity = new GenericAttribute { Id = 1 };

            // 将对象已有的属性(FirstName)的属性值设置为空值
            entity.SaveAttribute("FirstName", "", _genericAttributeService);

            _genericAttributeService.AssertWasCalled(x => x.SaveAttribute(Arg<GenericAttribute>.Matches(ga => ga.Id == 1 && ga.Value == "")));
        }

        [Fact(DisplayName = "确保扩展属性值为空时会删除扩展属性")]
        public void Ensure_delete_generic_attribute_via_entity_extensions_method2()
        {
            var entity = new GenericAttribute { Id = 1 };

            // 将对象已有的属性(FirstName)的属性值设置为空对象
            entity.SaveAttribute<string>("FirstName", null, _genericAttributeService);

            _genericAttributeService.AssertWasCalled(x => x.SaveAttribute(Arg<GenericAttribute>.Matches(ga => ga.Id == 1 && ga.Value == null)));
        }

        [Fact(DisplayName = "确保可以通过实体对象的扩展方法获取扩展属性")]
        public void Can_get_generic_attribute_via_entity_extensions_method()
        {
            var entity = new GenericAttribute { Id = 1 };

            var value = entity.GetAttribute<string>("FirstName", _genericAttributeService);

            value.Should().Be("Joy");

            var gender = entity.GetAttribute<bool>("gender", _genericAttributeService);
            gender.Should().BeTrue();
        }
    }
}
