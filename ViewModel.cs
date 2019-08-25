﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace BiliRaffle
{
    public class RelayCommand : ICommand

    {
        #region Private Fields

        private readonly Func<bool> _canExecute;

        private readonly Action _execute;

        #endregion Private Fields

        #region Public Constructors

        public RelayCommand(Action execute)

            : this(execute, null)

        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)

        {
            _execute = execute ?? throw new ArgumentNullException("execute");

            _canExecute = canExecute;
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler CanExecuteChanged

        {
            add

            {
                if (_canExecute != null)

                    CommandManager.RequerySuggested += value;
            }

            remove

            {
                if (_canExecute != null)

                    CommandManager.RequerySuggested -= value;
            }
        }

        #endregion Public Events

        #region Public Methods

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)

        {
            return _canExecute == null ? true : _canExecute();
        }

        public void Execute(object parameter)

        {
            _execute();
        }

        #endregion Public Methods
    }

    internal class ViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private static ViewModel _viewmodel;
        private bool _IsOneChance = true;
        private string _Msg;
        private int _Num = 1;
        private ICommand _Start;
        private string _Url;

        #endregion Private Fields

        #region Public Constructors

        public ViewModel()
        {
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// 单例实例
        /// </summary>
        public static ViewModel Main
        {
            get
            {
                if (_viewmodel == null)
                {
                    _viewmodel = new ViewModel();
                }
                return _viewmodel;
            }
            set
            {
                _viewmodel = value;
            }
        }

        /// <summary>
        /// 不统计重复
        /// </summary>
        public bool IsOneChance
        {
            get { return _IsOneChance; }
            set
            {
                _IsOneChance = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsOneChance"));
            }
        }

        /// <summary>
        /// 抽奖信息
        /// </summary>
        public string Msg
        {
            get { return _Msg; }
            set
            {
                _Msg = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Msg"));
            }
        }

        /// <summary>
        /// 中奖人数
        /// </summary>
        public int Num
        {
            get { return _Num; }
            set
            {
                _Num = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Num"));
            }
        }

        /// <summary>
        /// 开始抽奖命令
        /// </summary>
        public ICommand Start
        {
            get
            {
                if (_Start == null)
                {
                    _Start = new RelayCommand(() =>
                    {
                        if (string.IsNullOrEmpty(Url))
                        {
                            System.Windows.Forms.MessageBox.Show("抽奖地址不能为空！");
                            return;
                        }
                        Raffle.StartAsync(Url, Num, IsOneChance);
                        //Raffle.Start(Url, Num, IsOneChance);
                    });
                }
                return _Start;
            }
        }

        /// <summary>
        /// 抽奖地址
        /// </summary>
        public string Url
        {
            get
            {
                return _Url;
            }
            set
            {
                _Url = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Url"));
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// 推送信息
        /// </summary>
        /// <param name="msg">要推送的信息</param>
        public void PushMsg(string msg)
        {
            Msg += msg + "\r\n";
        }

        #endregion Public Methods
    }
}