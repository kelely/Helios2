using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Helios.Caching;
using Helios.Common.Services;
using Helios.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Helios.Common.Domain
{
    [TestFixture]
    public class SettingExtensionsTests
    {
        private ISettingService _settingService;
        private IRepository<Setting> _repository;

        [SetUp]
        public void Setup()
        {
            var settings = new List<Setting>
            {
                new Setting {Id = 1, TenantId = 0, Name = "FakeSettings.EnableOrderSystem", Value = "True"},
                new Setting {Id = 2, TenantId = 0, Name = "FakeSettings.ShortMessageServiceUrl", Value = "#"},

                new Setting {Id = 3, TenantId = 1, Name = "FakeSettings.ShortMessageServiceUrl", Value = "http://www.example.com/service"},

                new Setting {Id = 4, TenantId = 2, Name = "FakeSettings.EnableOrderSystem", Value = "False"},
                new Setting {Id = 5, TenantId = 2, Name = "FakeSettings.ShortMessageServiceUrl", Value = "http://www.example2.com/service"},
            };

            _repository = MockRepository.GenerateMock<IRepository<Setting>>();
            _repository.Stub(x => x.Table).Return(settings.AsQueryable());
            _repository.Stub(x => x.TableNoTracking).Return(settings.AsQueryable());

            _settingService = new SettingService(new HeliosNullCache(), _repository);
        }


        #region ISettings
        [Test(Description = "能够通过lambda表达式获取属性对应的key")]
        public void Can_get_setting_key_by_lambda_expression()
        {
            var setting = new FakeSettings();
            var key = setting.GetSettingKey(s => s.EnableOrderSystem);

            key.Should().Be("FakeSettings.EnableOrderSystem");
        }

        [Test(Description = "能够通过lambda表达式获取属性对应的key")]
        public void Can_convent_to_name_value_collection()
        {
            var setting = new FakeSettings();
            setting.EnableOrderSystem = true;
            setting.ShortMessageServiceUrl = "http://www.helios.com/service1";
            var collection = setting.AsDictionary();

            Assert.That(collection, Has.Count.EqualTo(2));
            Assert.That(collection["FakeSettings.EnableOrderSystem"], Is.EqualTo("True"));
            Assert.That(collection["FakeSettings.ShortMessageServiceUrl"], Is.EqualTo("http://www.helios.com/service1"));
        }
        #endregion

        #region ISettingService

        [Test]
        public void Test1()
        {
            var settings = _settingService.LoadSetting<FakeSettings>(1);

            Assert.That(settings, Is.Not.Null);
            Assert.That(settings.EnableOrderSystem, Is.True);
            Assert.That(settings.ShortMessageServiceUrl, Is.EqualTo("http://www.example.com/service"));
        }



        [Test(Description = "对应租户全新的配置可以全部保存到数据库")]
        public void Can_save_settings_collection()
        {
            var settings = new FakeSettings()
            {
                EnableOrderSystem = true,
                ShortMessageServiceUrl = "http://www.example.com/service3"
            };

            _settingService.SaveSettings(settings, 3);

            _repository.AssertWasCalled(x => x.Insert(Arg<Setting>.Is.Anything), options => options.Repeat.Times(2));
        }

        [Test(Description = "新的配置使用新增，老的配置使用更新, 不至于产生重复数据")]
        public void Can_save_settings_collection2()
        {
            var settings = new FakeSettings()
            {
                EnableOrderSystem = true,
                ShortMessageServiceUrl = "http://www.example.com/service3"
            };

            _settingService.SaveSettings(settings, 1);

            _repository.AssertWasCalled(x => x.Insert(Arg<Setting>.Is.Anything), options => options.Repeat.Times(1));
            _repository.AssertWasCalled(x => x.Update(Arg<Setting>.Is.Anything), options => options.Repeat.Times(1));
        }

        #endregion
    }
}
