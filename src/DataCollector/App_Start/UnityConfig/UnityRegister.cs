
using Mah.DataCollector.Entity.Entities;
using Mah.Common.Encrypt;
using Unity;
using Unity.AspNet.Mvc;
using Unity.Injection;
using DataAccess.Logic;
using Mah.Common.Logger;
using Mah.DataCollector.Service.Services.Log;
using Mah.DataCollector.Service.Services.Features;
using Mah.DataCollector.Interface.Interfaces.Features;

namespace DataCollector.App_Start.UnityConfig
{
    public static class UnityRegister
    {
        public static void RegisterTypes(IUnityContainer container)
        {
            string encryptionKey = "M@H&M@RZ!K3Y";
            Cryptor cryptor = new Cryptor(encryptionKey);
            string dbConnection = cryptor.Decrypt(System.Configuration.ConfigurationManager.ConnectionStrings["LogicDb"].ConnectionString);
            string gdbConnection = cryptor.Decrypt(System.Configuration.ConfigurationManager.ConnectionStrings["gdbConn"].ConnectionString);


            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<DataCollectorContext>(new PerRequestLifetimeManager(),
                            new InjectionConstructor(dbConnection)
                            );

            container.RegisterType<IFeatureService, FeatureService>(new PerRequestLifetimeManager(),
                            new InjectionConstructor(gdbConnection)
                            );

            container.RegisterType<ILogger, LoggService>();

            container.RegisterType<Cryptor>(new InjectionConstructor(encryptionKey));

            container.RegisterType<ClassesBL>(new InjectionConstructor(
                new ResolvedParameter(typeof(DataCollectorContext))
                , gdbConnection
                ));
            container.RegisterType<UpdateLogBL>();
            container.RegisterType<FeaturePicBL>();
            container.RegisterType<FieldsBL>();
            container.RegisterType<UserLocationBL>();
            container.RegisterType<DomainBL>();
            container.RegisterType<GISFeatureBL>(new InjectionConstructor(
                  new ResolvedParameter(typeof(DataCollectorContext)),
                    new ResolvedParameter(typeof(ClassesBL)),
                      new ResolvedParameter(typeof(FieldsBL)),
                        new ResolvedParameter(typeof(DomainBL))
                        , gdbConnection
                ));

            container.RegisterType<SettingBL>(
                new InjectionConstructor(
                  new ResolvedParameter(typeof(DataCollectorContext)),
                    new ResolvedParameter(typeof(Cryptor))
                        , gdbConnection
                ));
        }
    }
}