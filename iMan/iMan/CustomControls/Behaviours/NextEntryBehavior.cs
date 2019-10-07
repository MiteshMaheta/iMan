using System;
using Xamarin.Forms;

namespace iMan.CustomControls.Behaviours
{
    public class NextEntryBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty NextEntryProperty = BindableProperty.Create(nameof(NextEntry), typeof(Entry), typeof(Entry), defaultBindingMode: BindingMode.OneTime, defaultValue: null);

        public Entry NextEntry
        {
            get => (Entry)GetValue(NextEntryProperty);
            set => SetValue(NextEntryProperty, value);
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.Completed += Bindable_Completed;
            base.OnAttachedTo(bindable);
        }

        private void Bindable_Completed(object sender, EventArgs e)
        {
            if (NextEntry != null)
            {
                NextEntry.Focus();
            }
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.Completed -= Bindable_Completed;
            base.OnDetachingFrom(bindable);
        }
    }
}
