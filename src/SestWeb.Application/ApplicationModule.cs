using System;
using Autofac;
using SestWeb.Domain;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;
using SestWeb.Domain.Entities.Correlações.Base.CreatingCorrelação;
using SestWeb.Domain.Entities.Correlações.Base.EditingCorrelação;
using SestWeb.Domain.Entities.Correlações.Base.Validator;
using SestWeb.Domain.Entities.Correlações.ConstantesCorrelação.Validator;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.Validator;
using SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação.Validator;
using SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação.Validator;
using SestWeb.Domain.Entities.Correlações.VariáveisCorrelação.Validator;
using Module = Autofac.Module;

namespace SestWeb.Application
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            if (builder.Properties.ContainsKey(GetType().AssemblyQualifiedName))
            {
                return;
            }
            builder.Properties.Add(GetType().AssemblyQualifiedName, null);

            builder.RegisterModule<DomainModule>();
            //RegisterClassesWithPrivateConstructors(builder);

            // Registra todos os tipos do assembly SestWeb.Application
            builder.RegisterAssemblyTypes(typeof(ApplicationModule).Assembly)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

        }

        private void RegisterClassesWithPrivateConstructors(ContainerBuilder builder)
        {
            try
            {
                // Analitics
                //var analiticsCtor = typeof(Analitics).GetConstructor(
                //    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                //    null, Type.EmptyTypes, null);
                //var analitics = (Analitics)analiticsCtor.Invoke(null);
                //builder
                //    .Register(x => analitics).As<IAnalitics>()
                //    .AsSelf();

                //CorrelaçãoEmCriaçãoValidator
                ICorrelaçãoEmCriaçãoValidator correlaçãoEmCriaçãoValidator = new CorrelaçãoEmCriaçãoValidator();

                ICorrelaçãoEmEdiçãoValidator correlaçãoEmEdiçãoValidator = new CorrelaçãoEmEdiçãoValidator();

                // AutorValidator
                IAutorValidator autorValidator = new AutorValidator();

                // VariáveisValidator
                IVariáveisValidator variáveisValidator = new VariáveisValidator();

                // ConstantesValidator
                IConstantesValidator constantesValidator = new ConstantesValidator();

                // PerfisSaídaValidator
                IPerfisSaídaValidator perfisSaídaValidator = new PerfisSaídaValidator();

                // PerfisEntradaValidator
                IPerfisEntradaValidator perfisEntradaValidator = new PerfisEntradaValidator();

                // ExpressionValidator
                IExpressionValidator expressionValidator = new ExpressionValidator(variáveisValidator, constantesValidator, perfisSaídaValidator);

                // CorrelaçãoValidator
                ICorrelaçãoValidator CorrelaçãoValidator = new CorrelaçãoValidator(autorValidator, expressionValidator);

                //// PerfisEntradaFactory
                //var perfisEntradaFactoryCtor = typeof(PerfisEntradaFactory).GetConstructor(
                //    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                //    null, Type.EmptyTypes, null);
                //var perfisEntradaFactory = (PerfisEntradaFactory)perfisEntradaFactoryCtor.Invoke(null);
                //builder
                //    .Register(x => perfisEntradaFactory).As<IPerfisEntradaFactory>()
                //    .AsSelf();

                //// PerfisSaídaFactory
                //var perfisSaídaFactoryCtor = typeof(PerfisSaídaFactory).GetConstructor(
                //    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                //    null, Type.EmptyTypes, null);
                //var perfisSaídaFactory = (PerfisSaídaFactory)perfisSaídaFactoryCtor.Invoke(null);
                //builder
                //    .Register(x => perfisSaídaFactory).As<IPerfisSaídaFactory>()
                //    .AsSelf();

                //// VariáveisFactory
                //var variáveisFactoryCtor = typeof(VariáveisFactory).GetConstructor(
                //    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                //    null, Type.EmptyTypes, null);
                //var variáveisFactory = (VariáveisFactory)variáveisFactoryCtor.Invoke(null);
                //builder
                //    .Register(x => variáveisFactory).As<IVariáveisFactory>()
                //    .AsSelf();

                //// ConstantesFactory
                //var constantesFactoryCtor = typeof(ConstantesFactory).GetConstructor(
                //    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                //    null, Type.EmptyTypes, null);
                //var constantesFactory = (ConstantesFactory)constantesFactoryCtor.Invoke(null);
                //builder
                //    .Register(x => constantesFactory).As<IConstantesFactory>()
                //    .AsSelf();

                //// ExpressãoFactory
                //var expressãoFactoryCtor = typeof(ExpressãoFactory).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(IAnalitics), typeof(IVariáveisFactory), typeof(IConstantesFactory), typeof(IPerfisSaídaFactory), typeof(IPerfisEntradaFactory) }, null);
                //var expressãoFactory = (ExpressãoFactory)expressãoFactoryCtor.Invoke(new object[] { analitics, variáveisFactory, constantesFactory, perfisSaídaFactory, perfisEntradaFactory });
                //builder
                //    .Register(x => expressãoFactory).As<IExpressãoFactory>()
                //    .AsSelf();

                //var correlaçãoFactoryCtor = typeof(CorrelaçãoFactory).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(IExpressãoFactory), typeof(ICorrelaçãoEmCriaçãoValidator), typeof(ICorrelaçãoEmEdiçãoValidator), typeof(ICorrelaçãoValidator) }, null);
                //var correlaçãoFactory = (CorrelaçãoFactory)correlaçãoFactoryCtor.Invoke(new object[] { expressãoFactory, correlaçãoEmCriaçãoValidator, correlaçãoEmEdiçãoValidator, CorrelaçãoValidator });
                //builder
                //    .Register(x => correlaçãoFactory).As<ICorrelaçãoFactory>()
                //    .AsSelf();

                //// LoaderCorrelações
                //var loaderCorrelaçõesrCtor = typeof(LoaderCorrelações).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(ICorrelaçãoFactory) }, null);
                //var loaderCorrelações = (LoaderCorrelações)loaderCorrelaçõesrCtor.Invoke(new object[] { correlaçãoFactory });
                //builder
                //    .Register(x => loaderCorrelações).As<ILoaderCorrelações>()
                //    .AsSelf();

                //var container = builder.Build();

                //using (var scope = container.BeginLifetimeScope())
                //{
                //    var corrFactory = scope.Resolve<ICorrelaçãoFactory>();
                //    //var perfisEntrada = scope.Resolve<PerfisEntrada>();
                //    //var expressionAnalyser = scope.Resolve<ExpressionAnalyser>();
                //}

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}