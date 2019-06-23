﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using GTiff2Tiles.GUI.Resources;
using MaterialDesignExtensions.Controls;
using MaterialDesignThemes.Wpf;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.GUI.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// ViewModel for <see cref="Views.MainView"/>.
    /// </summary>
    public class MainViewModel : PropertyChangedBase
    {
        #region Properties

        #region UI

        /// <summary>
        /// Hint for InputFile TextBox.
        /// </summary>
        public string InputFileHint { get; } = Strings.InputFileHint;

        /// <summary>
        /// Hint for OutputDirectory TextBox.
        /// </summary>
        public string OutputDirectoryHint { get; } = Strings.OutputDirectoryHint;

        /// <summary>
        /// Hint for TempDirectory TextBox.
        /// </summary>
        public string TempDirectoryHint { get; } = Strings.TempDirectoryHint;

        /// <summary>
        /// Hint for MinZ TextBox.
        /// </summary>
        public string MinZHint { get; } = Strings.MinZHint;

        /// <summary>
        /// Hint for MaxZ TextBox.
        /// </summary>
        public string MaxZHint { get; } = Strings.MaxZHint;

        /// <summary>
        /// Hint for Algorithms ComboBox.
        /// </summary>
        public string AlgorithmsHint { get; } = Strings.AlgorithmsHint;

        /// <summary>
        /// Hint for ThreadsCount TextBox.
        /// </summary>
        public string ThreadsCountHint { get; } = Strings.ThreadsCountHint;

        /// <summary>
        /// Text in progress's TextBlock (e.g. "Progress:").
        /// </summary>
        public string ProgressTextBlock { get; } = Strings.ProgressTextBlock;

        /// <summary>
        /// Text inside Start button.
        /// </summary>
        public string StartButtonContent { get; } = Strings.StartButtonContent;

        /// <summary>
        /// Hey, it's me!
        /// </summary>
        public string Copyright { get; } = Enums.MainViewModel.Copyright;

        /// <summary>
        /// Assembly version.
        /// </summary>
        public string Version { get; } = Enums.MainViewModel.Version;

        #endregion

        #region TextBoxes/Blocks

        #region Private backing fields

        private int _threadsCount;

        private int _maxZ;

        private int _minZ;

        private string _inputFilePath;

        private string _outputDirectoryPath;

        private string _tempDirectoryPath;

        #endregion

        /// <summary>
        /// Threads count.
        /// </summary>
        public int ThreadsCount
        {
            get => _threadsCount;
            set
            {
                _threadsCount = value;
                NotifyOfPropertyChange(() => ThreadsCount);
            }
        }

        /// <summary>
        /// Maximum zoom.
        /// </summary>
        public int MaxZ
        {
            get => _maxZ;
            set
            {
                _maxZ = value;
                NotifyOfPropertyChange(() => MaxZ);
            }
        }

        /// <summary>
        /// Minimum zoom.
        /// </summary>
        public int MinZ
        {
            get => _minZ;
            set
            {
                _minZ = value;
                NotifyOfPropertyChange(() => MinZ);
            }
        }

        /// <summary>
        /// Input file path.
        /// </summary>
        public string InputFilePath
        {
            get => _inputFilePath;
            set
            {
                _inputFilePath = value;
                NotifyOfPropertyChange(() => InputFilePath);
            }
        }

        /// <summary>
        /// Output directory path.
        /// </summary>
        public string OutputDirectoryPath
        {
            get => _outputDirectoryPath;
            set
            {
                _outputDirectoryPath = value;
                NotifyOfPropertyChange(() => OutputDirectoryPath);
            }
        }

        /// <summary>
        /// Temp directory path.
        /// </summary>
        public string TempDirectoryPath
        {
            get => _tempDirectoryPath;
            set
            {
                _tempDirectoryPath = value;
                NotifyOfPropertyChange(() => TempDirectoryPath);
            }
        }

        #endregion

        #region ComboBox

        private string _algorithm;

        /// <summary>
        /// Currently chosen algorythm.
        /// </summary>
        public string Algorithm
        {
            get => _algorithm;
            set
            {
                _algorithm = value;
                NotifyOfPropertyChange(() => Algorithm);
            }
        }

        /// <summary>
        /// Collection of supported algorythms.
        /// </summary>
        // ReSharper disable once CollectionNeverQueried.Global
        public ObservableCollection<string> Algorithms { get; } = new ObservableCollection<string>();

        #endregion

        #region ProgressBar

        private double _progressBarValue;

        public double ProgressBarValue
        {
            get => _progressBarValue;
            set
            {
                _progressBarValue = value;
                NotifyOfPropertyChange(() => ProgressBarValue);
            }
        }

        #endregion

        private bool _isEnabled;

        /// <summary>
        /// Sets grid's state.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        /// <summary>
        /// Identifier of DialogHost on <see cref="Views.MainView"/>.
        /// </summary>
        public string DialogHostId { get; } = Enums.MainViewModel.DialogHostId;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize all needed properties.
        /// </summary>
        public MainViewModel()
        {
            InputFilePath = string.Empty;
            OutputDirectoryPath = string.Empty;
            TempDirectoryPath = string.Empty;
            MinZ = 0;
            MaxZ = 17;
            ThreadsCount = 5;
            ProgressBarValue = 0.0;
            IsEnabled = true;
            Algorithms.Add("crop");
            Algorithms.Add("join");
        }

        #endregion

        #region Methods

        #region Buttons

        /// <summary>
        /// Input directory button
        /// </summary>
        /// <returns></returns>
        public async ValueTask InputFileButton()
        {
            try
            {
                OpenFileDialogResult dialogResult =  await OpenFileDialog.ShowDialogAsync(Enums.MainViewModel.DialogHostId,
                                                                                          new OpenFileDialogArguments { CreateNewDirectoryEnabled = true });
                InputFilePath = dialogResult.Canceled ? InputFilePath : dialogResult.FileInfo.FullName;
            }
            catch (Exception exception)
            {
                await Helpers.ErrorHelper.ShowException(exception);
            }
        }

        /// <summary>
        /// Output directory button.
        /// </summary>
        /// <returns></returns>
        public async ValueTask OutputDirectoryButton()
        {
            try
            {
                OpenDirectoryDialogResult dialogResult =
                    await OpenDirectoryDialog.ShowDialogAsync(Enums.MainViewModel.DialogHostId,
                                                              new OpenDirectoryDialogArguments
                                                                  {CreateNewDirectoryEnabled = true});
                OutputDirectoryPath = dialogResult.Canceled ? OutputDirectoryPath : dialogResult.Directory;
            }
            catch (Exception exception)
            {
                await Helpers.ErrorHelper.ShowException(exception);
            }
        }

        /// <summary>
        /// Temp directory button.
        /// </summary>
        /// <returns></returns>
        public async ValueTask TempDirectoryButton()
        {
            try
            {
                OpenDirectoryDialogResult dialogResult =
                    await OpenDirectoryDialog.ShowDialogAsync(Enums.MainViewModel.DialogHostId,
                                                              new OpenDirectoryDialogArguments
                                                                  {CreateNewDirectoryEnabled = true});
                TempDirectoryPath = dialogResult.Canceled ? TempDirectoryPath : dialogResult.Directory;
            }
            catch (Exception exception)
            {
                await Helpers.ErrorHelper.ShowException(exception);
            }
        }

        /// <summary>
        /// Start button.
        /// </summary>
        /// <returns></returns>
        public async ValueTask StartButton()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            //Check properties for errors.
            if (!await CheckProperties()) return;

            //Initialize FileSystemEntries from properties.
            FileInfo inputFileInfo = new FileInfo(InputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(OutputDirectoryPath);

            //Create temp directory object.
            string tempDirectoryPath =
                Path.Combine(TempDirectoryPath, DateTime.Now.ToString(Core.Enums.DateTimePatterns.LongWithMs));
            DirectoryInfo tempDirectoryInfo = new DirectoryInfo(tempDirectoryPath);

            //Create progress reporter.
            IProgress<double> progress = new Progress<double>(value => ProgressBarValue = value);

            //Run tiling asynchroniously.
            try
            {
                //Check for errors.
                Core.Helpers.CheckHelper.CheckOutputDirectory(outputDirectoryInfo);
                if (!Core.Helpers.CheckHelper.CheckInputFile(inputFileInfo))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName, $"{Core.Enums.Image.Gdal.TempFileName}{Core.Enums.Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    Core.Image.Gdal.Warp(inputFileInfo, tempFileInfo, Core.Enums.Image.Gdal.RepairTifOptions);
                    inputFileInfo = tempFileInfo;
                }

                //Create image object.
                Core.Image.Image inputImage = new Core.Image.Image(inputFileInfo);

                //Switch on algorithm.
                switch (Algorithm)
                {
                    case Core.Enums.Algorithms.Join:
                        await inputImage.GenerateTilesByJoining(outputDirectoryInfo, MinZ, MaxZ, progress, ThreadsCount);
                        break;
                    case Core.Enums.Algorithms.Crop:
                        await inputImage.GenerateTilesByCropping(outputDirectoryInfo, MinZ, MaxZ, progress, ThreadsCount);
                        break;
                    default:
                        await Helpers.ErrorHelper.ShowError("This algorithm is not supported.", null);
                        IsEnabled = true;
                        return;
                }
            }
            catch (Exception exception)
            {
                await Helpers.ErrorHelper.ShowException(exception);
                IsEnabled = true;
                return;
            }

            //Enable controls.
            IsEnabled = true;

            stopwatch.Stop();
            await DialogHost.Show(new MessageBoxDialogViewModel($"{Strings.Done}! {Strings.TimePassed}:{Environment.NewLine}" +
                                                                $"{Strings.Days}:{stopwatch.Elapsed.Days} " +
                                                                $"{Strings.Hours}:{stopwatch.Elapsed.Hours} " +
                                                                $"{Strings.Minutes}:{stopwatch.Elapsed.Minutes} " +
                                                                $"{Strings.Seconds}:{stopwatch.Elapsed.Seconds} " +
                                                                $"{Strings.Milliseconds}:{stopwatch.Elapsed.Milliseconds}"));
        }

        #endregion

        #region Other

        /// <summary>
        /// Checks properties for errors and set some before starting.
        /// </summary>
        /// <returns><see langword="true"/> if no errors occured, <see langword="false"/> otherwise.</returns>
        private async ValueTask<bool> CheckProperties()
        {
            if (string.IsNullOrWhiteSpace(InputFilePath))
                return await Helpers.ErrorHelper.ShowError("Input file path is empty.", null);

            if (string.IsNullOrWhiteSpace(OutputDirectoryPath))
                return await Helpers.ErrorHelper.ShowError("Output directory path is empty.", null);

            if (string.IsNullOrWhiteSpace(TempDirectoryPath))
                return await Helpers.ErrorHelper.ShowError("Temp directory path is empty.", null);

            if (MinZ < 0)
                return await Helpers.ErrorHelper.ShowError("Minimum zoom is lesser, than 0.", null);

            if (MaxZ < 0)
                return await Helpers.ErrorHelper.ShowError("Maximum zoom is lesser, than 0.", null);

            if (MaxZ < MinZ)
                return await Helpers.ErrorHelper.ShowError("Minimum zoom is bigger, than maximum.", null);

            Algorithm = Algorithm.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(Algorithm))
                return await Helpers.ErrorHelper.ShowError("Please, choose the algorithm.", null);

            if (Algorithm != Core.Enums.Algorithms.Join && Algorithm != Core.Enums.Algorithms.Crop)
                return await Helpers.ErrorHelper.ShowError("This algorithm is not supported.", null);

            if (ThreadsCount <= 0)
                return await Helpers.ErrorHelper.ShowError("Threads count is lesser or equal 0.", null);

            //Disable controls.
            IsEnabled = false;

            //Set default progress bar value for each run.
            ProgressBarValue = 0.0;

            return true;
        }

        #endregion

        #endregion
    }
}
