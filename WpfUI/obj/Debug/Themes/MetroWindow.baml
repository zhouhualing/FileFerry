   M S B A M L   `   `   `  ����C  ?SampleUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:SampleUI.ControlsSampleUI.Controls  5      A%clr-namespace:GlowingWindow.BehaviorsGlowingWindow.Behaviors  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source'   !System.Windows.Shell.WindowChrome   DefaultChrome    ResizeBorderThickness    GlassFrameThickness    CornerRadius    CaptionHeight#   SampleUI.Controls.MetroWindow �� 
TargetType �� Property  ThemeBrushKey �� Value  ActiveForegroundBrushKey	 �� RelativeSource
 �� Path1   +GlowingWindow.Behaviors.MetroChromeBehavior  AccentBrushKey   ActiveBrush  BorderBrushKey   InactiveBrush   Mode �� 
TargetType J� SnapsToDevicePixels   DpiScaleTransform � LayoutTransform ��Name  ResizeGripIconElementKey  ForegroundBrushKey ! ResizeGripIconShadowElemenKey  BackgroundBrushKey �� Triggers P� Property P� Value�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      ,controlsclr-namespace:SampleUI.Controls  5      3	behaviors%clr-namespace:GlowingWindow.Behaviors  5        5      �� 5   
   $0 */SampleUI;component/Themes/ResizeGrip.xaml?�6   %�  &	        '  R     7   7  7     5
      $ 0q�5      $ 0q�5      $ 0��5      $ 0Q�5      �� 5      "  6   .�5   
   ��  � ��# �  5      �� 5   
    � ��# �  5      �� 5   
    � ��$ 0q�5      �� 5   
    �@  WindowChrome 5      �� 	 �� *Self+$
 WindowChrome��� 5   
    �@ WindowChrome8   5      �� 5   
    �@ MetroChromeBehavior 5        5      # �  60   # �  5      $ VisualStudio2013=�5      5!      �� 5"   
    � �� 5#      �� 5$      "  6"   .R�5%      �� #��z��6   #��z��5&      #��z��5'      # z 5(      ._�5)      �� .u�5*      � .+�5+   "   �� #��z��63   # z 5,      �� 5-   "   $ PART_ResizeGrip��6)   $��Bottom=�5.      $��Right=�5/      
������ ._�51   &   � $��16��6+   $��16��52      $��1q�53      .+�54   *   F� 8�� #���  55      F� 56   *   8�� #���  57      58   '   59   #   5:      5;      5<      5=      5?       5@   
   P� 5A       �@�WindowState$ 	Maximized=�5B      . �5C      ��  � ��$ 8q�5D      5E      5G      5I      