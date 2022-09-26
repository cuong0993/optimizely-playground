using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EPiServer.ServiceLocation;

namespace AlloyDemo.Business
{
    public class ServiceLocatorDependencyResolver : IDependencyResolver
    {
        private readonly IServiceLocator _serviceLocator;

        public ServiceLocatorDependencyResolver(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType.IsInterface || serviceType.IsAbstract) return GetInterfaceService(serviceType);
            return GetConcreteService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _serviceLocator.GetAllInstances(serviceType);
        }

        private object GetConcreteService(Type serviceType)
        {
            try
            {
                // Can't use TryGetInstance here because it won’t create concrete types
                return _serviceLocator.GetInstance(serviceType);
            }
            catch (ActivationException)
            {
                return null;
            }
        }

        private object GetInterfaceService(Type serviceType)
        {
            object instance;
            return _serviceLocator.TryGetExistingInstance(serviceType, out instance) ? instance : null;
        }
    }
}