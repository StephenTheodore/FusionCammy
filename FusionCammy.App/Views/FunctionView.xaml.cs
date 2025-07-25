﻿using FusionCammy.App.ViewModels;
using System.Windows.Controls;

namespace FusionCammy.App.Views
{
    /// <summary>
    /// DecoSelectionView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FunctionView : UserControl
    {
        public FunctionView(FunctionViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
