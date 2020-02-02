using Nancy.TinyIoc;
using System;
using System.Globalization;
using System.Reflection;
using Xamarin.Forms;

namespace Bot.ViewModels.Base
{
    public static class ViewModelLocator
    {

        private static TinyIoCContainer _container;

        static ViewModelLocator()
        {
            _container = new TinyIoCContainer();
        }

        public static readonly BindableProperty AutoWireViewModelProperty =
                  BindableProperty.CreateAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), default(bool), propertyChanged: OnAutoWireViewModelChanged);


        private static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as Element;
            if (view is null)
            {
                return;
            }

            var viewType = view.GetType();
            var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = string.Format(CultureInfo.InvariantCulture, $"{viewName}Model, {viewAssemblyName}");

            var viewModelType = Type.GetType(viewModelName);

            if (viewModelType is null)
            {
                return;
            }

            var viewModel = _container.Resolve(viewModelType);
            view.BindingContext = viewModel;
        }
        public static bool GetAutoWireViewModel(BindableObject bindable)
        {
            return (bool)bindable.GetValue(ViewModelLocator.AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(BindableObject bindable, bool value)
        {
            bindable.SetValue(ViewModelLocator.AutoWireViewModelProperty, value);
        }
    }
}
