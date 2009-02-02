﻿#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using Castle.Core.Configuration;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using SolrNet;

namespace Castle.Facilities.SolrNetIntegration.Tests {
    [TestFixture]
    public class Tests {
        [Test]
        [ExpectedException(typeof(FacilityException))]
        public void NoConfig_throws() {
            var container = new WindsorContainer();
            container.AddFacility<SolrNetFacility>();
        }

        [Test]
        [ExpectedException(typeof(FacilityException))]
        public void InvalidUrl_throws() {
            var configStore = new DefaultConfigurationStore();
            var configuration = new MutableConfiguration("solr");
            configuration.CreateChild("solrURL", "123");
            configStore.AddFacilityConfiguration("solr", configuration);
            var container = new WindsorContainer(configStore);
            container.AddFacility<SolrNetFacility>("solr");
        }

        [Test]
        [ExpectedException(typeof(FacilityException))]
        public void InvalidProtocol_throws() {
            var configStore = new DefaultConfigurationStore();
            var configuration = new MutableConfiguration("solr");
            configuration.CreateChild("solrURL", "ftp://localhost");
            configStore.AddFacilityConfiguration("solr", configuration);
            var container = new WindsorContainer(configStore);
            container.AddFacility<SolrNetFacility>("solr");
        }

        [Test]
        public void Ping_Query() {
            var configStore = new DefaultConfigurationStore();
            var configuration = new MutableConfiguration("solr");
            configuration.CreateChild("solrURL", "http://localhost:8983/solr");
            configStore.AddFacilityConfiguration("solr", configuration);
            var container = new WindsorContainer(configStore);
            container.AddFacility<SolrNetFacility>("solr");

            var locator = new WindsorServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);

            var solr = container.Resolve<ISolrOperations<Document>>();
            solr.Ping();
            Console.WriteLine(solr.Query(SolrQuery.All).Count);
        }

        public class Document {}
    }
}