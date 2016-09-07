using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helios.Caching;
using Helios.Common.Domain;
using Helios.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Helios.Common.Services
{
    [TestFixture]
    public class SettingServiceTests
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

        [Test(Description = "可以获取到所有的配置项")]
        public void Can_get_all_settings()
        {
            var settings = _settingService.GetAllSettings();
            Assert.That(settings, Has.Count.EqualTo(5));
        }

        [Test(Description = "可以根据配置的Key获取到配置项")]
        public void Can_get_setting_by_key()
        {
            // 在租户对应的配置项缺失时返回系统默认的配置
            var setting = _settingService.GetSetting("FakeSettings.EnableOrderSystem", 1, true);
            Assert.That(setting, Is.Not.Null);
            Assert.That(setting.Id, Is.EqualTo(1));
            Assert.That(setting.Value, Is.EqualTo("True"));

            // 在租户对应的配置项缺失时返回空
            setting = _settingService.GetSetting("FakeSettings.EnableOrderSystem", 1, false);
            Assert.That(setting, Is.Null);

            // 在租户对应的配置项存在时返回对应的配置数据
            setting = _settingService.GetSetting("FakeSettings.EnableOrderSystem", 2);
            Assert.That(setting, Is.Not.Null);
            Assert.That(setting.Id, Is.EqualTo(4));
            Assert.That(setting.Value, Is.EqualTo("False"));
        }

        [Test(Description = "可以根据Id获取到配置项")]
        public void Can_get_setting_by_id()
        {
            var setting = _settingService.GetSettingById(3);
            Assert.That(setting, Is.Not.Null);
            Assert.That(setting.Id, Is.EqualTo(3));
        }

        [Test(Description = "对应租户全新的配置可以全部保存到数据库")]
        public void Can_save_settings_collection()
        {
            var collection = new Dictionary<string, string>();
            collection.Add("FakeSettings.EnableOrderSystem", "True");
            collection.Add("FakeSettings.ShortMessageServiceUrl", "http://www.example.com/service3");

            _settingService.SaveSettings(collection, 3);

            _repository.AssertWasCalled(x => x.Insert(Arg<Setting>.Is.Anything), options => options.Repeat.Times(2));
        }

        [Test(Description = "新的配置使用新增，老的配置使用更新, 不至于产生重复数据")]
        public void Can_save_settings_collection2()
        {
            var collection = new Dictionary<string, string>();
            collection.Add("FakeSettings.EnableOrderSystem", "True");
            collection.Add("FakeSettings.ShortMessageServiceUrl", "http://www.example.com/service3");

            _settingService.SaveSettings(collection, 1);

            _repository.AssertWasCalled(x => x.Insert(Arg<Setting>.Is.Anything), options => options.Repeat.Times(1));
            _repository.AssertWasCalled(x => x.Update(Arg<Setting>.Is.Anything), options => options.Repeat.Times(1));
        }

        [Test(Description = "更新配置时, 值为空的配置项会被删除")]
        public void Can_save_settings_collection3()
        {
            var collection = new Dictionary<string, string>();
            collection.Add("FakeSettings.EnableOrderSystem", null);

            _settingService.SaveSettings(collection, 2);

            _repository.AssertWasCalled(x => x.Delete(Arg<Setting>.Is.Anything), options => options.Repeat.Times(1));
        }

        [Test(Description = "可以删除配置项")]
        public void Can_delete_setting()
        {
            _settingService.DeleteSetting("FakeSettings.EnableOrderSystem", 2);

            _repository.AssertWasCalled(x => x.Delete(Arg<Setting>.Is.Anything));
        }

        [Test(Description = "删除配置时, 不存在的配置将不会调用数据库删除操作")]
        public void Can_delete_setting2()
        {
            var collection = new NameValueCollection();
            collection.Add("FakeSettings.EnableOrderSystem", "");

            _settingService.DeleteSetting("FakeSettings.EnableOrderSystem", 1);

            _repository.AssertWasNotCalled(x => x.Delete(Arg<Setting>.Is.Anything));
        }
    }
}
