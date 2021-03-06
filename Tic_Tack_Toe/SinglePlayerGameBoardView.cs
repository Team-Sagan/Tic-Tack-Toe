﻿namespace Tic_Tack_Toe
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using Tic_Tack_Toe.EventArgs;
    using Tic_Tack_Toe.Interfaces;
    using Tic_Tack_Toe.Models;
    using Tic_Tack_Toe.Presenters;
    using Tic_Tack_Toe.Resources;

    public partial class SinglePlayerGameBoardView : Form, ISinglePlayerGameBoardView
    {
        private IMultiPlayerGameBoardPresenter presenter;

        public SinglePlayerGameBoardView()
        {
            this.InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            var firstPlayer = new HumanPlayer("You", "X") { IsOnTurn = true };
            var secondPlayer = new Computer("O");
            var gameBoardModel = new SinglePlayerGameRoomModel(firstPlayer, secondPlayer);
            this.presenter = new SinglePlayerGameBoardPresenter(this, gameBoardModel);
        }

        public event EventHandler<ComputerTurnEventArgs> ComputerTurn;

        public event EventHandler PlayerIconSet;

        public event EventHandler<ButtonEventArgs> PlayerMove;

        public IPlayer Player { get; set; }

        public void DisplayComputerTurn(int xCoordinate, int yCoordinate)
        {
            foreach (var btn in
                this.Controls.Cast<Button>()
                    .Where(btn => btn.Location.X == xCoordinate && btn.Location.Y == yCoordinate))
            {
                btn.Text = this.Player.PlayerSymbol;
            }
        }

        public void ResetGameBoard()
        {
            foreach (
                var button in
                    this.Controls.Cast<Button>()
                        .Where(button => button.Text != Messages.MainMenu && button.Text != Messages.Exit))
            {
                button.Text = string.Empty;
            }
        }

        void IMultiPlayerGameBoardView.GameDraw()
        {
            var result = MessageBox.Show(Messages.DrawMessage);
            if (result == DialogResult.OK)
            {
                this.ResetGameBoard();
            }
        }

        void IMultiPlayerGameBoardView.GameWon(IPlayer winner)
        {
            var result = MessageBox.Show(string.Format("{0} win!", winner.PlayerName), Messages.Congratulations);
            if (result == DialogResult.OK)
            {
                this.ResetGameBoard();
            }
        }

        private void ButtonClick(object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            if (button.Text == string.Empty)
            {
                this.OnPlayerMove(new ButtonEventArgs(button.Size.Height, button.Location.X, button.Location.Y));
                button.Text = this.Player.PlayerSymbol;
                this.OnPlayerIconSet();
                this.OnComputerTurn(new ComputerTurnEventArgs(button.Size.Height));
                this.OnPlayerIconSet();
                this.btnFocused.Focus();
            }
        }

        private void ButtonExitClicked(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void ButtonMainMenuClicked(object sender, System.EventArgs e)
        {
            var message = MessageBox.Show(
                Messages.PossibleProgressLoss, 
                Messages.Information, 
                MessageBoxButtons.OKCancel, 
                MessageBoxIcon.Exclamation);
            if (message == DialogResult.OK)
            {
                var gameEntry = new GameEntry();
                gameEntry.ShowDialog();
                this.Close();
            }
        }

        private void OnComputerTurn(ComputerTurnEventArgs sizeEventArgs)
        {
            if (this.ComputerTurn != null)
            {
                this.ComputerTurn(this, sizeEventArgs);
            }
        }

        private void OnPlayerIconSet()
        {
            if (this.PlayerIconSet != null)
            {
                this.PlayerIconSet(this, System.EventArgs.Empty);
            }
        }

        private void OnPlayerMove(ButtonEventArgs e)
        {
            if (this.PlayerMove != null)
            {
                this.PlayerMove(this, e);
            }
        }
    }
}