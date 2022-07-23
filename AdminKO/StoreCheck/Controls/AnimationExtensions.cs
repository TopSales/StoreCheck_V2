using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

internal static class AnimationExtensions
{
   internal enum TransitionSpeed
   {
      Instant = 0,
      Fast = 100,
      Normal = 200,
      Slow = 500,
      VerySlow = 3000
   }

   /// <summary>
   /// Toggles the opacity of a control.
   /// </summary>
   /// <param name="control">The control.</param>
   internal static void ToggleControlFade(this FrameworkElement control)
   {
      control.ToggleControlFade(TransitionSpeed.Normal, false);
   }

   /// <summary>
   /// Toggles the opacity of a control.
   /// </summary>
   /// <param name="control">The control.</param>
   /// <param name="speed">The speed.</param>
   internal static void ToggleControlFade(this FrameworkElement control, TransitionSpeed speed, bool Hide)
   {
      Storyboard storyboard = new Storyboard();
      TimeSpan duration = new TimeSpan(0, 0, 0, 0, (int)speed); //

      DoubleAnimation doubleAnimation = new DoubleAnimation { From = 1.0, To = 0.0, Duration = new Duration(duration) };
      if (control.Opacity == 0.0)
      {
         doubleAnimation = new DoubleAnimation { From = 0.0, To = 1.0, Duration = new Duration(duration) };
      }

      Storyboard.SetTargetName(doubleAnimation, control.Name);
      //Storyboard.SetTarget(animation, control);
      Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Opacity", 0));
      storyboard.Children.Add(doubleAnimation);

      if (Hide)
      {
         var _Two = new DiscreteObjectKeyFrame { KeyTime = duration, Value = (Visibility.Hidden) };
         var keyFrameAnimation = new ObjectAnimationUsingKeyFrames { BeginTime = new TimeSpan(0) };
         keyFrameAnimation.KeyFrames.Add(_Two);
         Storyboard.SetTargetProperty(keyFrameAnimation, new PropertyPath("Visibility"));
         Storyboard.SetTarget(keyFrameAnimation, control);
         storyboard.Children.Add(keyFrameAnimation);
      };

      storyboard.Begin(control);
   }

   internal static void ShowFade(this FrameworkElement control, TransitionSpeed speed)
   {
      Storyboard storyboard = new Storyboard();
      TimeSpan duration = new TimeSpan(0, 0, 0, 0, (int)speed); //

      DoubleAnimation doubleAnimation = new DoubleAnimation { From = 0.0, To = 1.0, Duration = new Duration(duration) };
      control.Opacity = 0.0;
      control.Visibility = Visibility.Visible;

      Storyboard.SetTargetName(doubleAnimation, control.Name);
      //Storyboard.SetTarget(animation, control);
      Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Opacity", 0));
      storyboard.Children.Add(doubleAnimation);
      storyboard.Begin(control);
   }

   internal static void HideFade(this FrameworkElement control, TransitionSpeed speed)
   {
      Storyboard storyboard = new Storyboard();
      TimeSpan duration = new TimeSpan(0, 0, 0, 0, (int)speed); //

      DoubleAnimation doubleAnimation = new DoubleAnimation { From = 1.0, To = 0.0, Duration = new Duration(duration) };
      control.Opacity = 1.0;

      Storyboard.SetTargetName(doubleAnimation, control.Name);
      //Storyboard.SetTarget(animation, control);
      Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Opacity", 0));
      storyboard.Children.Add(doubleAnimation);
      storyboard.Completed += (object sender, EventArgs e) =>
      {
         control.Visibility = Visibility.Hidden;
      };

      storyboard.Begin(control);
   }
}

