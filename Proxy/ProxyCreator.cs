using System.Net;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.Models;

namespace Proxy
{
    public partial class ProxyCreator : Form
    {
        private ProxyServer _proxyServer;
        private ExplicitProxyEndPoint _explicitEndPoint;
        int _port;
        public ProxyCreator()
        {
            InitializeComponent();
            _proxyServer = new ProxyServer();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_proxyServer.ProxyRunning)
            {
                StopProxy();
            }
            else
            {

                StartProxy();
            }
        }

        private void StartProxy()
        {
            if (string.IsNullOrWhiteSpace(portTextBox.Text))
            {
                MessageBox.Show("port number is required.");
                return;
            }
            _port = int.Parse(portTextBox.Text);
            try
            {
                // تعریف نقطه اتصال روی پورت 10810 برای تمام آی‌پی‌های شبکه
                _explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, _port, false);

                // اضافه کردن به سرور
                _proxyServer.AddEndPoint(_explicitEndPoint);

                // شروع به کار سرور
                _proxyServer.Start();

                // نکته مهم: این تنظیمات باعث می‌شود پروکسی روی خودِ ویندوز ست نشود
                // و فقط به درخواست‌های گوشی پاسخ دهد تا تداخلی با فورتی پیش نیاید
                _proxyServer.SetAsSystemHttpProxy(_explicitEndPoint);
                // اگر فورتی قطع شد، خط بالا را حذف کنید یا مقدارش را false بگیرید

                btnStart.Text = "Stop";
                btnStart.BackColor = Color.LightGreen;
                this.Text = $"Proxy Running on Port {_port}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در استارت پروکسی: {ex.Message}");
            }
        }

        private void StopProxy()
        {
            _proxyServer.Stop();

            // پاکسازی نقاط اتصال برای استارت مجدد
            _proxyServer.ProxyEndPoints.Clear();

            btnStart.Text = "Start";
            btnStart.BackColor = SystemColors.Control;
            this.Text = "Proxy Stopped";
        }

        // جلوگیری از باز ماندن پورت هنگام بستن برنامه
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_proxyServer.ProxyRunning)
            {
                _proxyServer.Stop();
            }
            base.OnFormClosing(e);
        }

        private void portTextBox_TextChanged(object sender, EventArgs e)
        {


        }
        // اضافه کردن یک لیبل برای نمایش آی‌پی‌ها
        private Label lblIPs;

        private void ProxyCreator_Load(object sender, EventArgs e)
        {
            // نمایش آی‌پی‌ها هنگام لود شدن فرم
            lblIPs.Text = "Local IPs:\n" + GetLocalIPAddresses();
        }


        private void InitializeComponent()
        {
            btnStart = new Button();
            portTextBox = new TextBox();
            lblPort = new Label();
            lblIPs = new Label();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(435, 289);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(112, 34);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // portTextBox
            // 
            portTextBox.Location = new Point(389, 177);
            portTextBox.Name = "portTextBox";
            portTextBox.Size = new Size(221, 31);
            portTextBox.TabIndex = 1;
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Location = new Point(318, 182);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(53, 25);
            lblPort.TabIndex = 2;
            lblPort.Text = "Port :";
            // 
            // lblIPs
            // 
            lblIPs.AutoSize = true;
            lblIPs.ForeColor = Color.Blue;
            lblIPs.Location = new Point(20, 20);
            lblIPs.Name = "lblIPs";
            lblIPs.Size = new Size(111, 25);
            lblIPs.TabIndex = 3;
            lblIPs.Text = "Finding IPs...";
            // 
            // ProxyCreator
            // 
            ClientSize = new Size(906, 489);
            Controls.Add(lblPort);
            Controls.Add(portTextBox);
            Controls.Add(btnStart);
            Controls.Add(lblIPs);
            Name = "ProxyCreator";
            Text = "PhoneNetBridge";
            Load += ProxyCreator_Load;
            ResumeLayout(false);
            PerformLayout();

        }
        private string GetLocalIPAddresses()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipList = host.AddressList
                .Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .Select(ip => ip.ToString())
                .ToList();

            return ipList.Count > 0
                ? string.Join(Environment.NewLine, ipList)
                : "آی‌پی یافت نشد";
        }

        private Button btnStart;
        private TextBox portTextBox;
        private Label lblPort;
    }
}