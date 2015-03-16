namespace CipherDecrypter
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// The main form of the application.
    /// </summary>
    public partial class Main : Form
    {
        /// <summary>
        /// The key to the cipher, once the message has been decrypted.
        /// </summary>
        private CipherKey cipherKey;

        /// <summary>
        /// A separate thread for running the decryption strategy.
        /// </summary>
        private BackgroundWorker decryptionWorker;

        /// <summary>
        /// Initializes a new instance of the <see cref="Main"/> class.
        /// </summary>
        public Main()
        {
            this.decryptionWorker = new BackgroundWorker();
            this.InitializeComponent();
        }

        private void Decrypt_Click(object sender, EventArgs e)
        {
            string cleanInput;
            SubstitutionSolver substitutionSolver;

            cleanInput = CipherTextTools.RemoveNonAlphaCharacters(this.Input.Text);

            if (cleanInput.Length < 30)
            {
                this.ErrorMessage.Text = "At least 30 characters are required.";
                return;
            }

            substitutionSolver = ((ComboBoxAlgorithmItem)this.Algorithm.SelectedItem).Algorithm;

            this.LoadingImage.Visible = true;
            this.Input.ReadOnly = true;

            if (this.decryptionWorker.IsBusy != true)
            {
                this.decryptionWorker.RunWorkerAsync(new object[] { cleanInput, substitutionSolver });
            }
        }

        private void DecryptionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SubstitutionSolver substitutionSolver;

            object[] arguments = (object[])e.Argument;
            substitutionSolver = (SubstitutionSolver)arguments[1];

            e.Result = substitutionSolver.Solve((string)arguments[0]);
        }

        private void DecryptionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.LoadingImage.Visible = false;
            this.Input.ReadOnly = false;

            this.cipherKey = (CipherKey)e.Result;

            // Convert the users text using the returned key.
            if (this.KeepFormatting.Checked)
            {
                this.Output.Text = this.cipherKey.DecryptText(this.Input.Text);
            }
            else
            {
                this.Output.Text = this.cipherKey.DecryptText(CipherTextTools.RemoveNonAlphaCharacters(this.Input.Text));
            }

            // Display the key to the user. The labels are stored in this array for convience.
            TextBox[] letterBoxes = new TextBox[]
            {
                this.TextA, this.TextB, this.TextC, this.TextD, this.TextE, this.TextF, this.TextG, this.TextH,
                this.TextI, this.TextJ, this.TextK, this.TextL, this.TextM, this.TextN, this.TextO, this.TextP, this.TextQ,
                this.TextR, this.TextS, this.TextT, this.TextU, this.TextV, this.TextW, this.TextX, this.TextY, this.TextZ,
            };

            char c = 'A';
            foreach (TextBox letterBox in letterBoxes)
            {
                letterBox.Text = this.cipherKey.DecryptLetter(c).ToString();
                c++;
            }

            this.ResultDetails.Visible = true;
            this.ConfidenceAmount.Text = CipherTextTools.DetermineConfidence(this.cipherKey.DecryptText(this.Input.Text)).ToString("P");
        }

        private void Input_TextChanged(object sender, EventArgs e)
        {
            this.CharacterCount.Text = this.Input.Text.Length.ToString();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            ComboBoxAlgorithmItem algorithm;

            this.LoadingImage.Image = Properties.Resources.loading;
            this.LoadingImage.Visible = false;

            this.decryptionWorker.WorkerReportsProgress = false;
            this.decryptionWorker.WorkerSupportsCancellation = false;
            this.decryptionWorker.DoWork += new DoWorkEventHandler(this.DecryptionWorker_DoWork);
            this.decryptionWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.DecryptionWorker_RunWorkerCompleted);

            algorithm = new ComboBoxAlgorithmItem();
            algorithm.Text = "Quadgram Hill-Climbing";
            algorithm.Algorithm = new QuadgramHillClimbSolver();
            this.Algorithm.Items.Add(algorithm);

            algorithm = new ComboBoxAlgorithmItem();
            algorithm.Text = "Jakobsen's Fast Algorithm";
            algorithm.Algorithm = new JakobsensSolver();
            this.Algorithm.Items.Add(algorithm);

           

            this.Algorithm.SelectedIndex = 0;
            this.ResultDetails.Visible = false;
        }

        /// <summary>
        /// A decryption algorithm as it exists in a combobox.
        /// </summary>
        public class ComboBoxAlgorithmItem
        {
            /// <summary>
            /// Gets or sets an instance of the substitution algorithm.
            /// </summary>
            public SubstitutionSolver Algorithm { get; set; }

            /// <summary>
            /// Gets or sets the text to display for this item as it exists in a ComboBox.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// Returns a string that represents the current object.
            /// </summary>
            /// <returns>A string that represents the current object.</returns>
            public override string ToString()
            {
                return this.Text;
            }
        }
    }
}