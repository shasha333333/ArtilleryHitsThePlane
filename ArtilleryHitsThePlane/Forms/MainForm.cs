using ArtilleryHitsThePlane.Entities;
using ArtilleryHitsThePlane.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArtilleryHitsThePlane
{
    public partial class MainForm : Form
    {
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        private TimeSpan elapsedGameTime = TimeSpan.Zero;
        private SoundPlayer backgroundMusicPlayer;

        private List<Plane> planes = new List<Plane>();
        private Plane plane;
        private Bullet bullet;
        private Cannon cannon;
        private Explosion explosion;

        private int totalPlanes;  // 用户选择的飞机总数
        private int destroyedPlanes = 0;  // 坠毁的飞机数量
        private bool gameOver = false; // 新增游戏结束标志
        private double angleInRadians;

        private Image planeImage;  // 新增一个 Image 对象用于存储飞机图像
        private Image planeDestroyedImage;  // 新增一个 Image 对象用于存储飞机毁灭图像
        private Image cannonImage;  // 新增一个 Image 对象用于存储大炮炮台图像
        private Image cannonBarrelImage;  // 新增一个 Image 对象用于存储大炮炮管图像
        private Image cobCannonImage;  // 新增一个 Image 对象用于存储大炮图像
        private Image boomImage;  // 新增一个 Image 对象用于存储爆炸图像


        private PictureBox pictureBox;

        private Thread gameThread;

        //private readonly Task initializationTask;

        public MainForm()
        {
            InitializeComponent();
            //initializationTask = LoadResourcesAsync();
            LoadResourcesAsync();
            InitializeUI();
            InitializePictureBox();
            InitializeEntities();

            InitializeTimer();
            InitializeThreads();

            this.KeyPreview = true;
            //this.KeyDown += MainForm_KeyDown;
            this.KeyPress += MainForm_KeyPress;
            this.FormClosing += MainForm_FormClosing;
            this.DoubleBuffered = true;
        }

        private async Task InitializeMusic()
        {
            try
            {
                //backgroundMusicPlayer = new SoundPlayer("../../Resources/Musics/-飞机场的10：30.wav");
                //backgroundMusicPlayer = new SoundPlayer(Properties.Resources.植物大战僵尸BGM);
                backgroundMusicPlayer = await Task.Run(() => new SoundPlayer("../../Resources/Musics/植物大战僵尸BGM.wav"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                backgroundMusicPlayer = await Task.Run(() => new SoundPlayer(Properties.Resources.植物大战僵尸BGM));
            }

        }

        private void PlayBackgroundMusic()
        {
            // 播放背景音乐
            backgroundMusicPlayer.PlayLooping();
        }

        private void StopBackgroundMusic()
        {
            // 停止背景音乐
            backgroundMusicPlayer.Stop();
        }

        private async Task InitializeImage()
        {
            try
            {
                await LoadImagesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task LoadImagesAsync()
        {
            await Task.Run(() =>
            {
                // 加载飞机图像
                planeImage = Properties.Resources.实心飞机;
                planeDestroyedImage = Properties.Resources.实心飞机__坠落_;

                // 加载大炮炮台图像
                cannonImage = Properties.Resources.cannon;

                // 加载大炮炮管图像
                cannonBarrelImage = Properties.Resources.connonBarrel;
                cannonBarrelImage.RotateFlip(RotateFlipType.Rotate270FlipNone);

                // 加载炮弹图像
                cobCannonImage = Properties.Resources.CobCannon_cob;
                cobCannonImage.RotateFlip(RotateFlipType.Rotate270FlipNone);

                // 加载炮弹图像
                boomImage = Properties.Resources.Boom;
            });
        }

        private async void LoadResourcesAsync()
        {
            await InitializeImage();
            await InitializeMusic();
        }

        private void InitializeThreads()
        {
            gameThread = new Thread(GameLoop)
            {
                IsBackground = true
            };
            gameThread.Start();
        }

        private async void GameLoop()
        {
            Stopwatch gameLoopTimer = new Stopwatch();
            gameLoopTimer.Start();

            while (!gameOver)
            {
                // 游戏逻辑处理
                UpdateGame();

                // 游戏渲染
                RenderGame();

                // 控制游戏循环速度
                int elapsedMilliseconds = (int)gameLoopTimer.ElapsedMilliseconds;
                int sleepTime = Math.Max(0, 16 - elapsedMilliseconds); // 60帧每秒
                await Task.Delay(sleepTime);

                gameLoopTimer.Restart();
            }
        }

        private void UpdateGame()
        {
            /*  // 更新飞机位置等逻辑
              plane.X += 1;

              if (plane.X > this.Width)
              {
                  plane.X = 0;
              }

              if (cannon.X != this.Width/2)
              {
                  cannon.X = this.Width / 2;
                  cannon.Y = this.Height ;
              }

              if (bullet.Y< 0 || bullet.Y> this.Height || bullet.X <0 || bullet.X > this.Width )
              {
                  bullet.X = this.Width / 2;
                  bullet.Y= this.Height;
              }

              if (cannon.OnFired)
              {
                  angleInRadians = cannon.Angle * Math.PI / 180.0;
                  bullet.X -= (int)(bullet.Speed * Math.Cos(angleInRadians));
                  bullet.Y -= (int)(bullet.Speed * Math.Sin(angleInRadians));


                  if (RectangleOverlapChecker.AreRectanglesOverlapping(plane.X,plane.Y,plane.Width,plane.Height,bullet.X,bullet.Y,bullet.Width,bullet.Height))
                  {
                      plane.Y += 20;
                  }
              }*/

            //窗口变化时炮台重置
            if (cannon.X != this.Width / 2 || cannon.Y != this.Height)
            {
                cannon.X = this.Width / 2;
                cannon.Y = this.Height;
            }

            //子弹超出边界重置
            if (bullet.Y < 0 || bullet.Y > this.Height || bullet.X < 0 || bullet.X > this.Width)
            {
                bullet.X = this.Width / 2;
                bullet.Y = this.Height;
            }

            //子弹坐标更新
            if (cannon.OnFired)
            {
                angleInRadians = cannon.Angle * Math.PI / 180.0;
                bullet.X -= (int)(bullet.Speed * Math.Cos(angleInRadians));
                bullet.Y -= (int)(bullet.Speed * Math.Sin(angleInRadians));
            }
            
            //多线程处理飞机位置逻辑
            Parallel.ForEach(planes, plane =>
            {
                lock(planes)
                {
                    if (plane.IsAlive)
                    {
                        plane.X += plane.Speed;

                        if (plane.X > this.Width)
                        {
                            plane.X = 0;
                        }

                        if (plane.Y > this.Height)
                        {
                            //plane.IsAlive = false;
                            //destroyedPlanes++;

                        }
                        //碰撞检查
                        if (RectangleOverlapChecker.AreRectanglesOverlapping(plane.X, plane.Y, plane.Width, plane.Height, bullet.X, bullet.Y, bullet.Width, bullet.Height))
                        {
                            //plane.Y += plane.DownSpeed;

                            // 发生碰撞，启动爆炸效果
                            explosion.X = plane.X;
                            explosion.Y = plane.Y;
                            explosion.IsActive = true;


                            //飞机坠落
                            plane.Y += this.Height;

                            //子弹刷新
                            bullet.X = this.Width / 2;
                            bullet.Y = this.Height;

                            if (plane.Y > this.Height && !gameOver)
                            {
                                plane.IsAlive = false;
                                destroyedPlanes++;

                            }

                        }
                    }
                }
            
            });

            if (destroyedPlanes == totalPlanes)
            {
                // 所有飞机坠毁，游戏结束
                GameOver();
            }

        }

        private void GameOver()
        {
            // 游戏结束的逻辑，例如显示游戏结束的消息、停止计时器等
            gameOver = true;
            timer.Stop();
            gameTimer.Stop();
            pictureBox.Invalidate();
        }

        private void RenderGame()
        {
            /*            if (plane.Y > this.Height && !gameOver)
                        {
                            // 触发游戏结算动画
                            gameOver = true;
                            gameTimer.Stop(); // 停止计时器
                            timer.Stop(); // 结束定时器
                            pictureBox.Invalidate();
                        }
            */

            Draw();
        }

        private void InitializePictureBox()
        {
            pictureBox = new PictureBox
            {
                Size = this.ClientSize,
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            pictureBox.Paint += PictureBox_Paint;
            this.Controls.Add(pictureBox);
        }

        private void InitializeUI()
        {
            this.Size = new Size(this.Width, this.Height);
            this.BackColor = Color.White;

            gameTimer.Interval = 1000; // 设置计时器间隔为1秒
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start(); // 启动计时器
        }

        private void InitializeEntities()
        {
            /*            plane = new Plane(1, true)
                        {
                            X = 0,
                            Y = this.Height / 2,
                            Width = Utils.Constants.PlaneWidth,
                            Height = Utils.Constants.PlaneHeight
                        };
            */
            // 创建飞机对象列表
            InitializePlanes();

            // 创建子弹对象
            bullet = new Bullet(this.Width / 2, this.Height, (int)(Utils.Constants.BulletSpeedDefault * this.Height))
            {
                Width = (int)(Utils.Constants.BulletWidth * this.Width),
                Height = (int)(Utils.Constants.BulletHeight * this.Height)
            };

            // 创建炮台对象
            cannon = new Cannon(this.Width / 2, this.Height)
            {
                Width = Utils.Constants.CannonWidth,
                Height = Utils.Constants.CannonHeight
            };

            // 创建 Explosion 对象
            explosion = new Explosion(0, 0, 10); // 这里的 10 是爆炸效果的帧数，根据实际情况调整

        }

        private void InitializePlanes()
        {
            totalPlanes = GetUserInputForPlaneCount();  // 获取用户选择的飞机数量
            Random random = new Random();
            for (int i = 0; i < totalPlanes; i++)
            {
                int minX = (int)(this.Width * Utils.Constants.PlaneWidth);
                int minY = (int)(this.Height * Utils.Constants.PlaneHeight);
                int maxX = this.Width - (int)(Utils.Constants.PlaneWidth * this.Width);
                int maxY = this.Height - (int)(this.Height * Utils.Constants.PlaneHeight);
                int randomInRangeX = random.Next(minX, maxX);
                int randomInRangeY = random.Next(minX, maxY);
                //Console.WriteLine($"X:{randomInRangeX}");
                //Console.WriteLine($"Y:{randomInRangeY}");
                Plane plane = new Plane()
                {
                    X = randomInRangeX,  // 水平方向间隔 150 像素
                    //X = planeX,
                    Y = randomInRangeY,
                    //Y = planeY,
                    Width = (int)(Utils.Constants.PlaneWidth * this.Width),
                    Height = (int)(Utils.Constants.PlaneHeight * this.Width),
                    Speed = (int)(Utils.Constants.PlaneSpeed * this.Width),
                    DownSpeed = (int)(Utils.Constants.PlaneDownSpeed * this.Height),
                };
                planes.Add(plane);
            }
        }

        private int GetUserInputForPlaneCount()
        {
            using (var planeCountForm = new PlaneCountForm())
            {
                var result = planeCountForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    totalPlanes = planeCountForm.SelectedPlaneCount;
                    return totalPlanes;
                }
                else
                {
                    // 用户取消输入或者关闭窗体，使用默认值
                    totalPlanes = Utils.Constants.PlaneNumMin;
                    return totalPlanes;
                }
            }
        }

        private void InitializeTimer()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 10;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            elapsedGameTime = elapsedGameTime.Add(TimeSpan.FromSeconds(1));

            // 将时间显示在控制台或者其他地方
            pictureBox.Invalidate();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // 用户输入监听
            // 这里可以添加对用户输入的处理
        }

        // ... 其他事件处理和方法

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // 绘制游戏时间
            DrawGameTime(g);

            // 绘制规则表格
            DrawRulesTable(g);

            //绘制爆炸效果
            if (explosion.IsActive)
            {
                // 根据爆炸效果的帧数和状态绘制爆炸效果
                g.DrawImage(boomImage, explosion.X, explosion.Y, bullet.Width * 10, bullet.Height * 10);

                // 在每一帧结束后，更新爆炸帧数
                explosion.ExplosionFrame++;
                if (explosion.ExplosionFrame >= explosion.ExplosionFrameCount)
                {
                    explosion.IsActive = false;
                    explosion.ExplosionFrame = 0;
                }
            }

            // 绘制炮弹
            if (cannon.OnFired)
            {
                g.DrawImage(cobCannonImage, bullet.X - bullet.Width / 2, bullet.Y - bullet.Height / 2, bullet.Width, bullet.Height);
            }

            // 绘制大炮
            DrawCannon(g);

            //绘制炮台
            g.DrawImage(cannonImage, cannon.X - cannon.Width / 2, cannon.Y - cannon.Height / 4, cannon.Width, cannon.Height / 2);

            // 绘制飞机
            foreach (var plane in planes)
            {
                if (plane.IsAlive)
                    g.DrawImage(planeImage, plane.X - plane.Width / 2, plane.Y - plane.Height / 2, plane.Width, plane.Height);
                //else
                //    g.DrawImage(planeDestroyedImage, plane.X - plane.Width / 2, plane.Y - plane.Height / 2, plane.Width, plane.Height);
            }

            //结束动画
            if (gameOver)
            {
                DrawGameOverAnimation(g);
            }
        }

        private void DrawCannon(Graphics g)
        {
            GraphicsState state = g.Save();
            g.TranslateTransform(cannon.X, cannon.Y); // 将坐标系移到大炮的底座中心
            g.RotateTransform(cannon.Angle); // 根据角度旋转坐标系
            g.DrawImage(cannonBarrelImage, -cannon.Width, -cannon.Height / 2, cannon.Width, cannon.Height); // 绘制旋转后的大炮
            g.Restore(state);
        }

        private void DrawGameTime(Graphics g)
        {
            // 设置游戏时间的相对字体大小
            float timeFontSizePercentage = 0.015f;
            int timeX = (int)(this.Width * timeFontSizePercentage);
            int timeY = (int)(this.Height * (1 - 2 * timeFontSizePercentage));
            string timeText = $"Time: {elapsedGameTime.ToString(@"hh\:mm\:ss")}";
            Font timeFont = new Font("Arial", this.Height * timeFontSizePercentage, FontStyle.Bold);
            g.DrawString(timeText, timeFont, Brushes.Black, timeX, timeY);
        }

        private void DrawRulesTable(Graphics g)
        {
            // 定义表格的位置和大小（相对定位）
            float tableXPercentage = 0.01f;  // 表格左上角距离窗体左侧的百分比
            float tableYPercentage = 0.02f;   // 表格左上角距离窗体顶部的百分比
            float cellWidthPercentage = 0.3f;  // 单元格宽度占窗体宽度的百分比
            float cellHeightPercentage = 0.04f; // 单元格高度占窗体高度的百分比

            // 根据百分比计算具体的位置和大小
            int tableX = (int)(this.Width * tableXPercentage);
            int tableY = (int)(this.Height * tableYPercentage);
            int cellWidth = (int)(this.Width * cellWidthPercentage);
            int cellHeight = (int)(this.Height * cellHeightPercentage);

            // 定义表头
            string[] headers = { "Rule", "Description" };

            // 定义规则
            string[,] rules = {
                {"操控说明", "wsad控制炮台角度和子弹速度方向"},
                {"炮台数值说明", $"炮台角度范围{Utils.Constants.CannonAngleMin}到{Utils.Constants.CannonAngleMax}，初始值为{Utils.Constants.CannonAngleDefault}"},
                {"子弹数值说明", $"子弹速度范围{Utils.Constants.BulletSpeedMin*this.Height}到{Utils.Constants.BulletSpeedMax*this.Height}，初始值为{Utils.Constants.BulletSpeedDefault*this.Height}"},
                {"飞机数值说明", $"飞机速度为{Utils.Constants.PlaneSpeed*this.Width}，被子弹命中后下降{Utils.Constants.PlaneDownSpeed*this.Height}"},
                // ... 添加其他规则
            };

            // 设置表头的相对字体大小
            float headerFontSizePercentage = 0.02f;
            Font headerFont = new Font("Arial", this.Height * headerFontSizePercentage, FontStyle.Bold);


            // 绘制表头
            for (int i = 0; i < headers.Length; i++)
            {
                g.FillRectangle(Brushes.LightGray, tableX + (int)(i * cellWidth), tableY, cellWidth, cellHeight);
                g.DrawRectangle(Pens.Black, tableX + (int)(i * cellWidth), tableY, cellWidth, cellHeight);
                g.DrawString(headers[i], headerFont, Brushes.Black, tableX + (int)(i * cellWidth) + 5, tableY + 5);
            }

            // 设置规则内容的相对字体大小
            float contentFontSizePercentage = 0.015f;
            Font contentFont = new Font("Arial", this.Height * contentFontSizePercentage);

            // 绘制规则内容
            for (int i = 0; i < rules.GetLength(0); i++)
            {
                for (int j = 0; j < rules.GetLength(1); j++)
                {
                    g.FillRectangle(Brushes.White, tableX + (int)(j * cellWidth), tableY + (int)((i + 1) * cellHeight), cellWidth, cellHeight);
                    g.DrawRectangle(Pens.Black, tableX + (int)(j * cellWidth), tableY + (int)((i + 1) * cellHeight), cellWidth, cellHeight);
                    g.DrawString(rules[i, j], contentFont, Brushes.Black, tableX + (int)(j * cellWidth) + 5, tableY + (int)((i + 1) * cellHeight) + 5);
                }
            }

            // 获取实时信息
            string bulletSpeedText = $"当前子弹速度: {bullet.Speed}";
            string cannonAngleText = $"当前炮台角度: {cannon.Angle}";
            string totalPlanesText = $"总飞机数量: {totalPlanes}";
            string destroyedPlanesText = $"已经击毁飞机数量: {destroyedPlanes}";



            // 设置实时信息的相对字体大小
            float infoFontSizePercentage = 0.015f;
            Font infoFont = new Font("Arial", this.Height * infoFontSizePercentage);


            // 绘制实时信息
            g.FillRectangle(Brushes.White, tableX, tableY + (int)(rules.GetLength(0) + 2) * cellHeight, cellWidth, cellHeight);
            g.DrawRectangle(Pens.Black, tableX, tableY + (int)(rules.GetLength(0) + 2) * cellHeight, cellWidth, cellHeight);
            g.DrawString(bulletSpeedText, contentFont, Brushes.Black, tableX + 5, tableY + (int)(rules.GetLength(0) + 2) * cellHeight + 5);

            g.FillRectangle(Brushes.White, tableX, tableY + (int)(rules.GetLength(0) + 3) * cellHeight, cellWidth, cellHeight);
            g.DrawRectangle(Pens.Black, tableX, tableY + (int)(rules.GetLength(0) + 3) * cellHeight, cellWidth, cellHeight);
            g.DrawString(cannonAngleText, contentFont, Brushes.Black, tableX + 5, tableY + (int)(rules.GetLength(0) + 3) * cellHeight + 5);

            g.FillRectangle(Brushes.White, tableX, tableY + (int)(rules.GetLength(0) + 5) * cellHeight, cellWidth, cellHeight);
            g.DrawRectangle(Pens.Black, tableX, tableY + (int)(rules.GetLength(0) + 5) * cellHeight, cellWidth, cellHeight);
            g.DrawString(totalPlanesText, contentFont, Brushes.Black, tableX + 5, tableY + (int)(rules.GetLength(0) + 5) * cellHeight + 5);

            g.FillRectangle(Brushes.White, tableX, tableY + (int)(rules.GetLength(0) + 6) * cellHeight, cellWidth, cellHeight);
            g.DrawRectangle(Pens.Black, tableX, tableY + (int)(rules.GetLength(0) + 6) * cellHeight, cellWidth, cellHeight);
            g.DrawString(destroyedPlanesText, contentFont, Brushes.Black, tableX + 5, tableY + (int)(rules.GetLength(0) + 6) * cellHeight + 5);
        }

        private void Draw()
        {
            pictureBox.Invalidate(); // 通知PictureBox重新绘制
        }

        private void DrawGameOverAnimation(Graphics g)
        {
            // 在这里添加游戏结算动画的绘制逻辑，例如文字、特效等
            string infoText = "You Win!";
            Font font = new Font("Arial", 20, FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Red);
            g.DrawString(infoText, font, brush, this.Width / 2, this.Height / 2);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 在窗口加载时开始播放背景音乐
            PlayBackgroundMusic();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 在窗口关闭时停止背景音乐
            StopBackgroundMusic();

            StopListening(); // 停止计时器等

            // 在窗口关闭时通知游戏循环线程退出
            DestroyThread(); // 销毁线程

            // 销毁资源
            DestroyResources();
        }

        private void StopListening()
        {
            // 停止监听的代码...
            // 例如，如果有一个计时器 timer，则可以使用 timer.Stop() 方法
            timer.Stop();
            gameTimer.Stop();
        }

        private void DestroyThread()
        {
            // 在游戏循环线程中进行退出逻辑，确保线程安全地退出
            gameOver = true;
            gameThread.Abort();
        }

        private void DestroyResources()
        {
            // 清理资源的代码，例如释放图片资源等
            // 可以使用 Dispose() 方法释放资源，或者手动设置为 null

            // 例如：
            planeImage?.Dispose();
            cannonImage?.Dispose();
            cannonBarrelImage?.Dispose();
            cobCannonImage?.Dispose();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (cannon.Angle - Utils.Constants.CannonAnglePart > Utils.Constants.CannonAngleMin)
                        cannon.Angle -= Utils.Constants.CannonAnglePart;
                    break;
                case Keys.Right:
                    if (cannon.Angle + Utils.Constants.CannonAnglePart < Utils.Constants.CannonAngleMax)
                        cannon.Angle += Utils.Constants.CannonAnglePart;
                    break;
                case Keys.Up:
                    if (bullet.Speed + Utils.Constants.BulletSpeedPart * this.Height < Utils.Constants.BulletSpeedMax * this.Height)
                        bullet.Speed += (int)(Utils.Constants.BulletSpeedPart * this.Height);
                    break;
                case Keys.Down:
                    if (bullet.Speed - Utils.Constants.BulletSpeedPart * this.Height > Utils.Constants.BulletSpeedMin * this.Height)
                        bullet.Speed -= (int)(Utils.Constants.BulletSpeedPart * this.Height);
                    break;
            }
            pictureBox.Invalidate();
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 键盘按下事件
            char keyPressed = e.KeyChar;

            switch (keyPressed)
            {
                case 'a':
                    if (cannon.Angle - Utils.Constants.CannonAnglePart > Utils.Constants.CannonAngleMin)
                        cannon.Angle -= Utils.Constants.CannonAnglePart;
                    break;
                case 'd':
                    if (cannon.Angle + Utils.Constants.CannonAnglePart < Utils.Constants.CannonAngleMax)
                        cannon.Angle += Utils.Constants.CannonAnglePart;
                    break;
                case 'w':
                    //cannonFired = true;
                    if (bullet.Speed + Utils.Constants.BulletSpeedPart * this.Height < Utils.Constants.BulletSpeedMax * this.Height)
                        bullet.Speed += (int)(Utils.Constants.BulletSpeedPart * this.Height);
                    break;
                case 's':
                    //cannonFired = false;
                    if (bullet.Speed - Utils.Constants.BulletSpeedPart * this.Height > Utils.Constants.BulletSpeedMin * this.Height)
                        bullet.Speed -= (int)(Utils.Constants.BulletSpeedPart * this.Height);
                    break;
            }
            pictureBox.Invalidate();
        }



        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (backgroundMusicPlayer != null)
            {
                if (backgroundMusicPlayer.IsLoadCompleted)
                {
                    PlayBackgroundMusic();
                }
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (backgroundMusicPlayer != null)
            {
                if (backgroundMusicPlayer.IsLoadCompleted)
                {
                    StopBackgroundMusic();
                }
            }

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
