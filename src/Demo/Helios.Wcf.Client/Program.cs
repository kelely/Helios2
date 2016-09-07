using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helios.Common;
using Helios.Common.Domain;
using Helios.Common.Services;
using Helios.Infrastructure;

namespace Helios.Wcf.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            EngineContext.Initialize(false);

            #region IGenericAttributeService
            // TODO: 完成 IGenericAttributeService 的演示代码
            var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();

            //            genericAttributeService.SaveAttribute();
            //            genericAttributeService.GetAttributeById(1);

            #endregion

            #region ISettingService
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            var setting = new FakeSetting();
            setting.EnableOrderSystem = false;
            setting.ShortMessageServiceUrl = "http://www.example.com/sms";
            settingService.SaveSettings(setting, 1);

            var setting2 = settingService.LoadSetting<FakeSetting>(1);

            #endregion
        }
    }
}
