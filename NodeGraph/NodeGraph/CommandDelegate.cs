using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApplication1
{
	public class CommandDelegate : ICommand
	{
		/// <summary>
		/// 現在の状態でこのコマンドを実行できるかどうかを判断するメソッドを定義します。
		/// </summary>
		private Func<object, bool> canExecute_;

		/// <summary>
		/// 現在の状態でこのコマンドを実行できるかどうかを判断するメソッドを定義します。
		/// </summary>
		private Action<object> execute_;

		/// <summary>
		/// コマンドの起動時に実行するメソッドを指定してインスタンスを生成、初期化します。
		/// </summary>
		/// <param name="execute">コマンドの起動時に実行するメソッド</param>
		public CommandDelegate(Action<object> execute)
            : this(execute, o => true)
        {
		}

		/// <summary>
		/// コマンドの起動時に実行するメソッドとコマンドを実行するかどうかを返すメソッドを指定してインスタンスを生成、初期化します。
		/// </summary>
		/// <param name="execute">コマンドの起動時に実行するメソッド</param>
		/// <param name="canExecute">コマンドを実行するかどうかを返すメソッド</param>
		public CommandDelegate(Action<object> execute, Func<object, bool> canExecute)
		{
			execute_ = execute;
			canExecute_ = canExecute;
		}

		/// <summary>
		/// コマンドを実行するかどうかに影響するような変更があった場合に発生します。
		/// </summary>
		event EventHandler ICommand.CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		/// <summary>
		/// 現在の状態でこのコマンドを実行できるかどうかを返します。
		/// </summary>
		/// <param name="parameter">コマンドで使用されたデータ。 コマンドにデータを渡す必要がない場合は、このオブジェクトを null に設定されます。 </param>
		/// <returns>このコマンドを実行できる場合は true。それ以外の場合は false。</returns>
		bool ICommand.CanExecute(object parameter)
		{
			return canExecute_(parameter);
		}

		/// <summary>
		/// コマンドの起動時に呼び出されます。
		/// </summary>
		/// <param name="parameter">コマンドで使用されたデータ。 コマンドにデータを渡す必要がない場合は、このオブジェクトを null に設定されます。</param>
		void ICommand.Execute(object parameter)
		{
			execute_(parameter);
		}
	}
}
