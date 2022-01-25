using LispMachine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MacroProcessor
{
	public partial class MainForm : Form
	{
		private string input_file;
		private string direct;
		public MainForm()
		{
			direct = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
			input_file = direct + "\\source.txt";
			InitializeComponent();
			this.Show();
			this.Activate();
		}

		
		private void btn_first_run_Click(object sender, EventArgs e)
		{
			tb_error.Clear();
			tb_result.Clear();
			StartREPL(new StringReader(tb_source_code.Text));

		}


		#region 

		private void fm_main_Load(object sender, EventArgs e)
		{
			this.tb_input_file.Text = this.input_file;
			disableButtons();

			if (!String.IsNullOrEmpty(this.tb_input_file.Text))
			{
				try
				{
					fillSourceTextBoxFromFile(this.tb_input_file.Text, this.tb_source_code);
					enableButtons();
					this.tb_error.Clear();
				}
				catch (Exception)
				{
					this.tb_error.Text = "Не удалось загрузить данные с файла. Возможно путь к файлу указан неверно";
					disableButtons();
				}
			}
		}

		private void btn_load_Click(object sender, EventArgs e)
		{
			try
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.Filter = "Text Files (.txt)|*.txt";
				ofd.InitialDirectory = direct;
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					try
					{
						this.tb_source_code.Clear();
						this.tb_input_file.Text = ofd.FileName;
						enableButtons();
						fillSourceTextBoxFromFile(ofd.FileName, this.tb_source_code);
					}
					catch (Exception)
					{
						throw new Exception("Не удалось загрузить данные с файла. Возможно путь к файлу указан неверно");
					}
				}
			}
			catch (Exception ex)
			{
				this.tb_error.Text = ex.Message;
				disableButtons();
			}
		}

		private void btn_save_Click(object sender, EventArgs e)
		{
			try
			{
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.Filter = "Text Files (.txt)|*.txt";
				sfd.InitialDirectory = direct;
				if (sfd.ShowDialog() == DialogResult.OK)
				{
					this.tb_input_file.Text = sfd.FileName;
					List<String> temp = tb_result.Text.Split('\n').ToList<String>();
					StreamWriter sw = new StreamWriter(sfd.FileName);
					foreach (string str in temp)
					{
						sw.WriteLine(str);
					}
					sw.Close();
				}
			}
			catch (Exception)
			{
				this.tb_error.Text = "Не удалось загрузить данные с файла. Возможно путь к файлу указан неверно";
				disableButtons();
			}
		}

		private void enableButtons()
		{
			this.btn_first_run.Enabled = true;
		}


		private void disableButtons()
		{
			this.btn_first_run.Enabled = false;
		}



		private void fillSourceTextBoxFromFile(string file, RichTextBox tb)
		{
				String temp = String.Empty;
				StreamReader sr = new StreamReader(file);
				while ((temp = sr.ReadLine()) != null)
				{
					tb.AppendText(temp + Environment.NewLine);
				}
				sr.Close();
		}


		private void StartREPL (TextReader reader)
		{
			SExprParser replParser;
			try
			{ replParser = new SExprParser(reader);
			}
			catch (LexerException e)
			{
				tb_error.Text = "Lexer error:"+e.Message;
				return; }

			SExpr replExpr;

			try
			{
				SExpr evaluated = null;
				while ((replExpr = replParser.GetSExpression()) != null)
				{
					replExpr.PrintSExpr();
					evaluated = Evaluator.Evaluate(replExpr);
				}

				//оцениваем только последнее выражение из серии, если надо все - внести в цикл
				if (evaluated != null)
					tb_result.Text = evaluated.GetText();
				else
					MessageBox.Show("null");

			}
			catch (LexerException e)
			{
				tb_error.Text = "Lexer error:" + e.Message;
			}
			catch (ParserException e)
			{
				tb_error.Text = "Can't parse:" + e.Message;
			}
			catch (MacroException e)
			{
				tb_error.Text = "Macro expansion error: " + e.Message;
			}
			catch (EvaluationException e)
			{
				tb_error.Text = "Can't evaluate:" + e.Message;
			}
			catch (Exception e)
			{
				tb_error.Text = "Some other error:" + e.Message;
			}
		}

		#endregion

	}
}
