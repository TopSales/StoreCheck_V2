using ZPF;
using ZPF.XF.Compos;

public class ColorViewModel : BaseViewModel, IColorModel
{
   public enum ColorModes { Dark, Light }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   static ColorViewModel _Current = null;
   internal Style ActionTilesStyle;

   public static ColorViewModel Current
   {
      get
      {
         if (_Current == null)
         {
            _Current = new ColorViewModel();
         };

         return _Current;
      }

      set
      {
         _Current = value;
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public ColorViewModel()
   {
      _Current = this;

      SetDark();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public void SetDark()
   {
      ColorMode = ColorModes.Dark;

      ToolbarColor = Microsoft.Maui.Graphics.Colors.Black;

      BackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb("FF262626");
      BackgroundColor50 = Microsoft.Maui.Graphics.Colors.Silver;

      TextColor = Microsoft.Maui.Graphics.Colors.White;
      TextColor80 = Microsoft.Maui.Graphics.Colors.Gray;

      HighLightColor = Microsoft.Maui.Graphics.Color.FromArgb("FF558ed5");

      ActionTextColor = Microsoft.Maui.Graphics.Colors.White;
      ActionBackgroundColor = HighLightColor;

      PostItColor = Microsoft.Maui.Graphics.Color.FromArgb("FFFFA5");
   }

   public void SetLight()
   {
      ColorMode = ColorModes.Light;

      ToolbarColor = Microsoft.Maui.Graphics.Colors.LightGray;

      BackgroundColor = Microsoft.Maui.Graphics.Colors.White;
      BackgroundColor50 = Microsoft.Maui.Graphics.Colors.Silver;

      TextColor = Microsoft.Maui.Graphics.Colors.Black;
      TextColor80 = Microsoft.Maui.Graphics.Colors.Gray;

      HighLightColor = Microsoft.Maui.Graphics.Color.FromArgb("FF558ed5");

      ActionTextColor = Microsoft.Maui.Graphics.Colors.White;
      ActionBackgroundColor = HighLightColor.MultiplyAlpha(0.8f);

      PostItColor = Microsoft.Maui.Graphics.Color.FromArgb("FFFFA5");
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public ColorModes ColorMode { get; private set; }

   #region IColorModel

   public Microsoft.Maui.Graphics.Color ToolbarColor { get; set; } = Microsoft.Maui.Graphics.Colors.White; // Black; // White

   public Microsoft.Maui.Graphics.Color BackgroundColor { get; set; } = Microsoft.Maui.Graphics.Colors.White; // Black; // White
   public Microsoft.Maui.Graphics.Color BackgroundColor50 { get; set; } = Microsoft.Maui.Graphics.Colors.Silver;

   public Microsoft.Maui.Graphics.Color TextColor { get; set; } = Microsoft.Maui.Graphics.Colors.Black; // White; // Black
   public Microsoft.Maui.Graphics.Color TextColor80 { get; set; } = Microsoft.Maui.Graphics.Colors.Gray;

   public Microsoft.Maui.Graphics.Color HighLightColor { get; set; } = Microsoft.Maui.Graphics.Colors.Orange;

   public Microsoft.Maui.Graphics.Color ActionTextColor { get; set; } = Microsoft.Maui.Graphics.Colors.Black;
   public Microsoft.Maui.Graphics.Color ActionBackgroundColor { get; set; } = Microsoft.Maui.Graphics.Colors.Orange;

   public Microsoft.Maui.Graphics.Color PostItColor { get; set; } = Microsoft.Maui.Graphics.Color.FromArgb("FFFFA5");

   #endregion

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public ResourceDictionary SetStyles()
   {
      var Resources = new ResourceDictionary();

      //Resources.Add(new Style(typeof(AttachmentList))
      //{
      //   Setters = {
      //              new Setter {
      //                  Property = Entry.BackgroundColorProperty,
      //                  Value = Microsoft.Maui.Graphics.Colors.Transparent,
      //              }
      //          }
      //});

      Resources.Add(new Style(typeof(Button))
      {
         Setters = {
                    new Setter {
                        Property = Button.TextColorProperty,
                        Value = ColorViewModel.Current.TextColor,
                    },
                    //new Setter {
                    //    Property = Button.FontSizeProperty,
                    //    Value = 24,
                    //},
                    new Setter {
                        Property = Button.CornerRadiusProperty,
                        Value = 0,
                    },
                    new Setter {
                        Property = Button.BackgroundColorProperty,
                        Value = ColorViewModel.Current.ActionBackgroundColor,
                    }
                }
      });

      //Resources.Add(new Style(typeof(ButtonEx))
      //{
      //   Setters = {
      //              new Setter {
      //                  Property = ButtonEx.TextColorProperty,
      //                  Value = ColorViewModel.Current.ActionTextColor,
      //              },
      //              new Setter {
      //                  Property = ButtonEx.BorderColorProperty,
      //                  Value = ColorViewModel.Current.ActionTextColor,
      //              },
      //              new Setter {
      //                  Property = ButtonEx.CornerRadiusProperty,
      //                  Value = 0,
      //              },
      //              new Setter {
      //                  Property = ButtonEx.BackgroundColorProperty,
      //                  Value = ColorViewModel.Current.ActionBackgroundColor,
      //              }
      //          }
      //});

      Resources.Add(new Style(typeof(Entry))
      {
         Setters = {
                    new Setter {
                        Property = Entry.TextColorProperty,
                        Value = ColorViewModel.Current.TextColor,
                    },
                    new Setter {
                        Property = Entry.PlaceholderColorProperty,
                        Value = ColorViewModel.Current.TextColor80,
                    },
                    new Setter {
                        Property = Entry.BackgroundColorProperty,
                        Value = Microsoft.Maui.Graphics.Colors.Transparent,
                    }
                }
      });

      //Resources.Add(new Style(typeof(EntryEx))
      //{
      //   Setters = {
      //              new Setter {
      //                  Property = EntryEx.TextColorProperty,
      //                  Value = ColorViewModel.Current.TextColor,
      //              },
      //              new Setter {
      //                  Property = EntryEx.PlaceholderColorProperty,
      //                  Value = ColorViewModel.Current.TextColor80,
      //              },
      //              new Setter {
      //                  Property = EntryEx.BackgroundColorProperty,
      //                  Value = Microsoft.Maui.Graphics.Colors.Transparent,
      //              }
      //          }
      //});

      Resources.Add(new Style(typeof(Label))
      {

         Setters = {
                    new Setter {
                        Property = Label.TextColorProperty,
                        Value = ColorViewModel.Current.TextColor80,
                    },
                    new Setter {
                        Property = Label.BackgroundColorProperty,
                        Value = Microsoft.Maui.Graphics.Colors.Transparent,
                    }
                }
      });

      //Resources.Add(new Style(typeof(LabelEx))
      //{
      //   Setters = {
      //              new Setter {
      //                  Property = Label.TextColorProperty,
      //                  Value = ColorViewModel.Current.TextColor,
      //              },
      //              new Setter {
      //                  Property = Label.BackgroundColorProperty,
      //                  Value = Microsoft.Maui.Graphics.Colors.Transparent,
      //              }
      //          }
      //});

      //Resources.Add(new Style(typeof(Panorama))
      //{
      //   Setters = {
      //              new Setter {
      //                  Property = Panorama.TextColorProperty,
      //                  Value = ColorViewModel.Current.TextColor,
      //              },
      //              new Setter {
      //                  Property = Panorama.BackgroundColorProperty,
      //                  Value = Microsoft.Maui.Graphics.Colors.Transparent,
      //              }
      //          }
      //});

      Resources.Add(new Style(typeof(Picker))
      {
         Setters = {
                    new Setter {
                        Property = Picker.TitleColorProperty,
                        Value = ColorViewModel.Current.TextColor80,
                    },
                    new Setter {
                        Property = Picker.TextColorProperty,
                        Value = ColorViewModel.Current.TextColor,
                    },
                    new Setter {
                        Property = Picker.BackgroundColorProperty,
                        Value = Microsoft.Maui.Graphics.Colors.Transparent,
                    }
                }
      });

        //Resources.Add(new Style(typeof(PickerEx))
        //{
        //   Setters = {
        //              new Setter {
        //                  Property = Picker.TitleColorProperty,
        //                  Value = ColorViewModel.Current.TextColor80,
        //              },
        //              new Setter {
        //                  Property = Picker.TextColorProperty,
        //                  Value = ColorViewModel.Current.TextColor,
        //              },
        //              new Setter {
        //                  Property = Picker.BackgroundColorProperty,
        //                  Value = Microsoft.Maui.Graphics.Colors.Transparent,
        //              }
        //          }
        //});

        //Resources.Add(new Style(typeof(BindablePicker))
        //{
        //   Setters = {
        //              new Setter {
        //                  Property = Picker.TitleColorProperty,
        //                  Value = ColorViewModel.Current.TextColor80,
        //              },
        //              new Setter {
        //                  Property = Picker.TextColorProperty,
        //                  Value = ColorViewModel.Current.TextColor,
        //              },
        //              new Setter {
        //                  Property = Picker.BackgroundColorProperty,
        //                  Value = Microsoft.Maui.Graphics.Colors.Transparent,
        //              }
        //          }
        //});

        Resources.Add(ActionTilesStyle = new Style(typeof(Tile))
        {
            Setters = {
                    new Setter {
                        Property = Tile.TextColorProperty,
                        Value = ColorViewModel.Current.TextColor,
                    },
                    new Setter {
                        Property = Tile.FontSizeProperty,
                        Value = 24,
                    },
                    new Setter {
                        Property = Tile.CornerRadiusProperty,
                        Value = 5,
                    },
                    //new Setter {
                    //    Property = Tile.BadgeColorProperty,
                    //    Value = Microsoft.Maui.Graphics.Color.DarkRed,
                    //},
                    //new Setter {
                    //    Property = Tile.ImageScaleProperty,
                    //    Value = 1,
                    //},
                    new Setter {
                        Property = Tile.BackgroundColorProperty,
                        Value = ColorViewModel.Current.ActionBackgroundColor,
                    }
                }
        });

        return Resources;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
