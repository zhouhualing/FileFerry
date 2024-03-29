﻿
using System;
using System.ComponentModel.Design;

namespace WD.Library.Core
{
    /// <summary>
    /// Registers a service in a service container.
    /// </summary>
    /// <attribute name="id" use="required">
    /// The service interface type.
    /// </attribute>
    /// <attribute name="class" use="required">
    /// The implementing service class name.
    /// </attribute>
    /// <usage>Only in /Application/Services</usage>
    /// <returns>
    /// <c>null</c>. The service is registered, but not returned.
    /// </returns>
    public class ServiceDoozer : IDoozer
	{
		public bool HandleConditions {
			get { return false; }
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			var container = (IServiceContainer)args.Parameter;
			if (container == null)
				throw new InvalidOperationException("Expected the parameter to be a service container");
			Type interfaceType = args.AddIn.FindType(args.Codon.Id);
			if (interfaceType != null) {
				string className = args.Codon.Properties["class"];
				bool serviceLoading = false;
				// Use ServiceCreatorCallback to lazily create the service
				container.AddService(
					interfaceType, delegate {
						// This callback runs within the service container's lock
						if (serviceLoading)
							throw new InvalidOperationException("Found cyclic dependency when initializating " + className);
						serviceLoading = true;
						return args.AddIn.CreateObject(className);
					});
			}
			return null;
		}
	}
}
