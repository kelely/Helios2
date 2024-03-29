﻿using Autofac;
using Common.Logging;
using Helios.Configuration;
using Helios.Infrastructure;
using Helios.Infrastructure.DependencyManagement;

namespace Helios.Data.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private static readonly ILog Logger = LogManager.GetLogger<DependencyRegistrar>();

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, HeliosConfig config)
        {
            Logger.Debug("注册数据持久化仓库 IRepository<>");

            // TODO: 配置文件中读取连接字符串
            builder.Register<IDbContext>(c => new HeliosObjectContext("Database=helios_entity_extensions;Data Source=localhost;User Id=meiyuner;Pwd=Js9zMr36HxJxUU7C;pooling=false;CharSet=utf8mb4;port=3306;Persist Security Info=True;")).InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
        }

        public int Order => -998;
    }
}