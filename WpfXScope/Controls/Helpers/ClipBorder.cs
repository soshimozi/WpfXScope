using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfXScope.Controls.Helpers
{
    /// <summary>
    /// Border which allows Clipping to its border.
    /// Useful especially when you need to clip to round corners.
    /// </summary>
    public class ClipBorder : Border
    {
        #region Fields

        private Geometry _clipRect;
        private object _oldClip;

        #endregion

        #region Overrides

        protected override void OnRender(DrawingContext dc)
        {
            OnApplyChildClip();
            base.OnRender(dc);
        }

        public override UIElement Child
        {
            get
            {
                return base.Child;
            }
            set
            {
                if (Child != value)
                {
                    if (Child != null)
                    {
                        // Restore original clipping of the old child
                        Child.SetValue(ClipProperty, _oldClip);
                    }

                    _oldClip = value != null ? value.ReadLocalValue(ClipProperty) : null;

                    base.Child = value;
                }
            }
        }

        #endregion

        #region Helpers

        protected virtual void OnApplyChildClip()
        {
            var child = Child;
            if (child == null) return;
            // Get the geometry of a rounded rectangle border based on the BorderThickness and CornerRadius
            _clipRect = GeometryHelper.GetRoundRectangle(new Rect(Child.RenderSize), BorderThickness, CornerRadius);
            child.Clip = _clipRect;
        }

        #endregion
    }
}
