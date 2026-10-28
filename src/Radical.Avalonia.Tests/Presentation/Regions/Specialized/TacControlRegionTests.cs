using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Presentation;
using Radical.Avalonia.Regions;
using Radical.Avalonia.Tests;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Test.Radical.Avalonia.Presentation.Regions.Specialized
{
    [TestClass]
    public class TacControlRegionTests
    {
        class TestTabControlRegion : TabControlRegion
        {
            public TestTabControlRegion()
            {
                TestHostingView = new Window();
            }

            public AvaloniaObject TestHostingView { get; set; }

            protected override AvaloniaObject FindHostingViewOf( Control fe )
            {
                return TestHostingView;
            }

            protected override object TryGetViewModel( AvaloniaObject view )
            {
                return ( ( Control )view ).DataContext;
            }
        }

        class TestViewModel : AbstractViewModel, IExpectViewActivatedCallback
        {

            public void OnViewActivated()
            {
                Invoked = true;
            }

            public bool Invoked { get; private set; }
        }

        class HardCodedServiceProvider : IServiceProvider
        {
            readonly IProvideValueTarget ipvt = new IPVT()
            {
                TargetObject = new TabControl()
            };

            public HardCodedServiceProvider()
            {

            }

            public object GetService( Type serviceType )
            {
                return serviceType == typeof( IProvideValueTarget ) ? ipvt : null;
            }
        }

        class IPVT : IProvideValueTarget
        {
            public object TargetObject
            {
                get;
                set;
            }

            public object TargetProperty
            {
                get;
                set;
            }
        }

        [AvaloniaTestMethod, TestCategory( "TabControlRegion" ), TestCategory( "Regions" ), TestCategory( "UI Composition" )]
        public void TabControlRegion_ActiveContentChanged_should_notify_VM_if_IExpectViewActivatedCallback()
        {
            var sut = new TestTabControlRegion();
            sut.ProvideValue(new HardCodedServiceProvider());

            var vm1 = new TestViewModel();
            var item1 = new UserControl()
            {
                DataContext = vm1
            };
            sut.Add(item1);

            var vm2 = new TestViewModel();
            var item2 = new UserControl()
            {
                DataContext = vm2
            };
            sut.Add(item2);

            sut.Activate(item2);
            sut.Activate(item1);

            Assert.IsTrue(vm1.Invoked);
            Assert.IsTrue(vm2.Invoked);
        }
        
        [AvaloniaTestMethod, TestCategory( "TabControlRegion" ), TestCategory( "Regions" ), TestCategory( "UI Composition" )]
        public void TabControlRegion_when_PreserveViewDataContext_true_should_assign_view_ViewModel_to_header_data_context()
        {
            var sut = new TestTabControlRegion();
            sut.ProvideValue(new HardCodedServiceProvider());

            var vm = new TestViewModel();
            var tabView = new UserControl()
            {
                DataContext = vm
            };
            
            var headerView = new TextBlock();
            
            RegionHeaderedElement.SetHeader(tabView, headerView);
            RegionHeaderedElement.SetPreserveOwningRegionDataContext(tabView, true);
            
            sut.Add(tabView);
            
            Assert.AreEqual(vm, headerView.DataContext);
        }
        
        [AvaloniaTestMethod, TestCategory( "TabControlRegion" ), TestCategory( "Regions" ), TestCategory( "UI Composition" )]
        public void TabControlRegion_when_PreserveViewDataContext_false_should_not_set_header_data_context()
        {
            var sut = new TestTabControlRegion();
            sut.ProvideValue(new HardCodedServiceProvider());

            var vm = new TestViewModel();
            var tabView = new UserControl()
            {
                DataContext = vm
            };
            
            var headerView = new TextBlock()
            {
                DataContext = new TestViewModel()
            };
            
            RegionHeaderedElement.SetHeader(tabView, headerView);
            RegionHeaderedElement.SetPreserveOwningRegionDataContext(tabView, false);
            
            sut.Add(tabView);
            
            Assert.AreNotEqual(vm, headerView.DataContext);
        }
        
        [AvaloniaTestMethod, TestCategory( "TabControlRegion" ), TestCategory( "Regions" ), TestCategory( "UI Composition" )]
        public void TabControlRegion_PreserveViewDataContext_defaults_to_false_and_should_not_set_header_data_context()
        {
            var sut = new TestTabControlRegion();
            sut.ProvideValue(new HardCodedServiceProvider());

            var vm = new TestViewModel();
            var tabView = new UserControl()
            {
                DataContext = vm
            };
            
            var headerView = new TextBlock()
            {
                DataContext = new TestViewModel()
            };
            
            RegionHeaderedElement.SetHeader(tabView, headerView);
            //PreserveOwningRegionDataContext defaults to false
            //RegionHeaderedElement.SetPreserveOwningRegionDataContext(tabView, false);
            
            sut.Add(tabView);
            
            Assert.AreNotEqual(vm, headerView.DataContext);
        }
    }
}
