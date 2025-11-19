# 零基础Unity完全教程 - 秦陵兵马俑ARPG

> 本教程专为Unity零基础用户编写，每一步都有详细说明和截图指引

---

## 📚 目录

1. [安装Unity](#第一步安装unity)
2. [创建/打开项目](#第二步打开项目)
3. [Unity界面介绍](#第三步认识unity界面)
4. [创建测试场景](#第四步创建测试场景)
5. [配置玩家角色](#第五步创建玩家角色)
6. [配置敌人](#第六步创建敌人)
7. [运行游戏](#第七步运行游戏)
8. [常见问题](#常见问题解决)

---

## 第一步：安装Unity

### 1.1 下载Unity Hub

**Unity Hub是什么？**
> Unity Hub是Unity的启动器和管理工具，就像是一个"游戏启动器"，可以管理多个Unity版本。

**下载步骤：**

1. 打开浏览器，访问：https://unity.com/download
2. 点击绿色的 **"Download Unity Hub"** 按钮
3. 下载完成后，双击安装包进行安装
4. 安装过程一路点击 **"下一步"** 即可

**安装位置建议：**
```
Windows: C:\Program Files\Unity Hub
Mac: /Applications/Unity Hub.app
```

---

### 1.2 安装Unity编辑器

**打开Unity Hub后：**

1. **创建账号（如果没有）**
   - 点击右上角的 "Sign in"（登录）
   - 点击 "Create account"（创建账号）
   - 填写邮箱、密码，按照指引完成注册
   - 免费的Personal版本就够用了！

2. **安装Unity编辑器**
   - 点击左侧 **"Installs"**（安装）标签
   - 点击右上角蓝色的 **"Install Editor"**（安装编辑器）按钮
   - 选择 **"Unity 2021.3.XX LTS"**（推荐版本）
     - LTS = Long Term Support（长期支持版本），最稳定
   - 点击 **"Next"**（下一步）

3. **选择模块（重要！）**
   勾选以下模块：
   ```
   ✅ Visual Studio Community (代码编辑器)
   ✅ Documentation (文档，可选)
   ✅ Language Pack 中文 (中文界面，可选)
   ```

4. **开始安装**
   - 点击 **"Done"**（完成）
   - 等待下载和安装（大约需要20-40分钟，取决于网速）
   - 安装完成后会在 "Installs" 列表中显示

---

### 1.3 获取免费许可证

**为什么需要许可证？**
> Unity需要激活才能使用，但Personal版本是完全免费的！

**激活步骤：**

1. 打开Unity Hub，确保已登录
2. 点击右上角的齿轮图标（设置）
3. 点击 **"License Management"**（许可证管理）
4. 点击 **"Activate New License"**（激活新许可证）
5. 选择 **"Unity Personal"**（Unity个人版）
6. 勾选 **"I don't use Unity in a professional capacity"**
   （我不在专业场合使用Unity）
7. 点击 **"Done"**（完成）

✅ 完成！现在Unity已经可以使用了！

---

## 第二步：打开项目

### 2.1 下载项目代码

**如果你还没有项目文件：**

```bash
# 方法1: 使用Git（推荐）
git clone <你的项目仓库地址>

# 方法2: 直接下载ZIP
1. 在GitHub/GitLab上找到项目
2. 点击绿色的 "Code" 按钮
3. 点击 "Download ZIP"
4. 解压到你喜欢的位置（如：D:\Projects\TerracottaARPG）
```

**项目文件夹结构应该是这样的：**
```
TerracottaARPG/
├── Assets/
├── README.md
├── QUICKSTART.md
└── 其他文件...
```

---

### 2.2 在Unity Hub中添加项目

1. 打开 **Unity Hub**
2. 点击左侧 **"Projects"**（项目）标签
3. 点击右上角蓝色的 **"Add"**（添加）按钮旁的下拉箭头
4. 选择 **"Add project from disk"**（从磁盘添加项目）
5. 浏览并选择项目的**根目录**
   ```
   选择：D:\Projects\TerracottaARPG
   （包含Assets文件夹的那个目录）
   ```
6. 点击 **"选择文件夹"**

---

### 2.3 打开项目

1. 在Unity Hub的项目列表中，找到 **"Terracotta ARPG"**
2. 点击项目名称打开
3. **第一次打开会比较慢**（可能需要5-15分钟）
   - Unity正在导入所有资源
   - 看到底部进度条在动就是正常的
   - 喝杯茶，耐心等待 ☕

---

## 第三步：认识Unity界面

### 3.1 Unity编辑器主要窗口

打开项目后，你会看到这样的界面：

```
┌─────────────────────────────────────────────────────────┐
│  菜单栏: File  Edit  Assets  GameObject  Component...   │
├──────────┬─────────────────────────────┬────────────────┤
│          │                             │                │
│ Hierarchy│       Scene View            │   Inspector    │
│ (层级)   │      (场景视图)              │   (检视器)     │
│          │                             │                │
│  场景中  │    可视化编辑游戏场景          │  选中对象的    │
│  的所有  │    可以拖拽、旋转、缩放        │  详细属性      │
│  对象列表│                             │                │
│          ├─────────────────────────────┤                │
│          │       Game View             │                │
│          │      (游戏视图)              │                │
│          │    运行游戏时显示的画面        │                │
├──────────┴─────────────────────────────┴────────────────┤
│               Project (项目窗口)                         │
│           显示所有资源文件（脚本、图片、音频等）            │
│  Assets/                                                │
│  ├── Scripts/                                           │
│  ├── Scenes/                                            │
│  └── ...                                                │
└─────────────────────────────────────────────────────────┘
```

### 3.2 重要窗口说明

**Hierarchy（层级窗口）- 左侧**
- 显示当前场景中的所有游戏对象
- 就像是"演员列表"
- 可以创建、删除、重命名对象

**Scene View（场景视图）- 中间上方**
- 可视化编辑场景的地方
- 可以移动、旋转、缩放对象
- **鼠标操作：**
  - **鼠标右键拖拽**：旋转视角
  - **鼠标中键拖拽**：平移视角
  - **滚轮**：缩放视角
  - **Alt + 左键拖拽**：围绕对象旋转

**Game View（游戏视图）- 中间下方**
- 显示游戏运行时玩家看到的画面
- 点击上方的"Game"标签可以切换到这里

**Inspector（检视器）- 右侧**
- 显示选中对象的所有属性和组件
- 可以修改对象的位置、旋转、大小等
- 可以添加/删除组件

**Project（项目窗口）- 底部**
- 显示所有项目文件
- 脚本、图片、音频、预制体等都在这里
- 类似于"文件资源管理器"

---

### 3.3 如果窗口消失了怎么办？

**不用担心！可以重新打开：**

1. 点击顶部菜单栏 **Window**（窗口）
2. 找到想要打开的窗口名称：
   - **Hierarchy** → Window > General > Hierarchy
   - **Inspector** → Window > General > Inspector
   - **Project** → Window > General > Project
   - **Scene** → Window > General > Scene

**恢复默认布局：**
- 点击右上角 **"Default"** 旁边的下拉菜单
- 选择 **"Revert Factory Settings"**（恢复出厂设置）

---

## 第四步：创建测试场景

### 4.1 创建新场景

1. **点击菜单栏：File > New Scene**
2. 在弹出的窗口中，选择 **"2D (URP)"** 模板
   ```
   为什么选2D？
   因为我们的游戏是2D视角的ARPG！

   URP是什么？
   Universal Render Pipeline（通用渲染管线）
   是现代Unity推荐的渲染方式
   ```
3. 点击 **"Create"**（创建）

---

### 4.2 保存场景

**重要：立即保存场景！**

1. 点击菜单栏：**File > Save Scene**
2. 在弹出的窗口中：
   - 左侧导航到 **Assets/Scenes** 文件夹
     - **如果没有Scenes文件夹，就创建一个：**
       - 右键 Assets 文件夹
       - 选择 **Create > Folder**
       - 命名为 **"Scenes"**
   - 文件名输入：**TestScene**
   - 点击 **"保存"**

✅ 场景创建完成！现在Hierarchy中应该有：
- Main Camera（主摄像机）
- 可能还有一个Global Volume或Light 2D

---

### 4.3 设置摄像机

**选中Main Camera（点击Hierarchy中的Main Camera）**

在右侧Inspector中，找到 **Camera组件**，设置：

```
【Camera组件】
- Projection: Orthographic （正交投影，2D游戏必须）
- Size: 8 （视野大小，可以调整）
- Background: 黑色
  - 点击Background旁边的颜色方块
  - 把颜色调成纯黑 (R:0, G:0, B:0, A:255)
```

**为什么用Orthographic？**
> Perspective（透视）= 3D游戏，近大远小
> Orthographic（正交）= 2D游戏，没有透视效果

---

## 第五步：创建玩家角色

### 5.1 创建系统管理器对象

**为什么先创建管理器？**
> 游戏需要一些"幕后工作者"来管理音效、镜头震动等功能

**1. 创建GameManager**

```
步骤：
1. 在Hierarchy空白处右键
2. 选择 "Create Empty"（创建空对象）
3. 在Inspector顶部，改名为：GameManager
4. 点击Inspector底部的 "Add Component"（添加组件）
5. 在搜索框输入：GameManager
6. 点击搜索结果中的 "Game Manager (Script)"
```

**2. 创建AudioHub（同样的方法）**

```
1. Hierarchy右键 > Create Empty
2. 改名：AudioHub
3. Add Component > 搜索：AudioHub
4. 点击 "Audio Hub (Script)"
```

**3. 创建CameraShaker**

```
1. Hierarchy右键 > Create Empty
2. 改名：CameraShaker
3. Add Component > 搜索：CameraShaker
4. 点击 "Camera Shaker (Script)"
```

**配置CameraShaker：**
- 在Inspector中找到 **Main Camera** 字段
- 把Hierarchy中的 **Main Camera** 拖到这个字段里
  ```
  方法：
  1. 在Hierarchy中点击Main Camera（不松开鼠标）
  2. 拖到Inspector的Main Camera字段上
  3. 松开鼠标
  ```

✅ 完成后Hierarchy应该是这样：
```
Main Camera
GameManager
AudioHub
CameraShaker
```

---

### 5.2 创建玩家角色对象

**1. 创建玩家精灵对象**

```
步骤：
1. Hierarchy右键 > 2D Object > Sprites > Square
   （创建一个方块精灵，临时代替玩家模型）
2. 改名为：Player
```

**2. 设置玩家位置**

选中Player，在Inspector的**Transform**组件中：
```
Position:
  X: 0
  Y: 0
  Z: 0

Rotation:
  X: 0
  Y: 0
  Z: 0

Scale:
  X: 1
  Y: 1
  Z: 1
```

**3. 设置玩家颜色（方便识别）**

在Inspector找到 **Sprite Renderer** 组件：
```
- Sprite: Square (默认)
- Color: 绿色
  点击Color旁的颜色方块，选择亮绿色
  (R:0, G:255, B:0)
```

---

### 5.3 添加物理组件

**为什么需要物理组件？**
> Rigidbody2D = 让对象有物理特性（可以移动、碰撞）
> Collider2D = 碰撞体积，定义对象的边界

**1. 添加Rigidbody2D**

```
选中Player > Inspector底部 > Add Component
搜索：Rigidbody2D
点击 "Rigidbody 2D"

配置：
Body Type: Dynamic （动态物体）
Gravity Scale: 0 （重力为0，因为是俯视角游戏）

展开Constraints:
✅ 勾选 Freeze Rotation Z （冻结旋转，防止玩家自己转圈）
```

**2. 添加CircleCollider2D**

```
Add Component > 搜索：CircleCollider2D
点击 "Circle Collider 2D"

配置：
Radius: 0.5 （碰撞半径）
```

---

### 5.4 添加玩家控制脚本

**最重要的一步！**

```
1. 选中Player
2. Add Component
3. 搜索：TerracottaWarrior
4. 点击 "Terracotta Warrior (Script)"
```

**配置TerracottaWarrior组件：**

在Inspector中，你会看到很多参数。逐个设置：

```
【Stats】
Max Health: 1000
Spirit Energy: 100
Max Spirit Energy: 100
Critical Chance: 0.15
Critical Multiplier: 2

【Clay Armor】
Base Damage Reduction: 0.25
Clay Layers: 3
Damage Buff Per Layer: 0.05

【Visuals & Audio】
Body Renderer: 拖入Player自己（Sprite Renderer组件会自动识别）
Clay Chips VFX: (暂时留空)
Spirit Light: (暂时留空)

【Shield Wall (W)】
Shield Duration: 1.2
Shield DR: 0.4
Reflect Percent: 0.5
Heavy Stun Seconds: 1.5
Shield Cooldown: 5

【Slam (Q)】
Slam Radius: 3
Slam Damage: 300
Slam Slow Duration: 1.5
Slam Slow Percent: 0.4
Slam Cooldown: 3
Damage Layer: 选择 "Everything" （稍后配置Layer）

【Execute (E)】
Execute Range: 2
Execute Health Threshold: 0.3
Execute Base Damage: 200
Execute Health Percent Damage: 0.2

【Movement】
Move Speed: 5
```

---

### 5.5 设置玩家Tag和Layer

**什么是Tag和Layer？**
> Tag = 标签，用于识别对象类型（如"玩家"、"敌人"）
> Layer = 层级，用于物理碰撞检测

**设置Tag：**
```
1. 选中Player
2. Inspector顶部找到 "Tag" 下拉框
3. 如果没有Player标签：
   - 选择 "Add Tag..."
   - 点击 "+" 号
   - 输入：Player
   - 回到Player对象，Tag选择"Player"
```

**设置Layer：**
```
1. 选中Player
2. Inspector顶部找到 "Layer" 下拉框
3. 选择 "Add Layer..."
4. 在 User Layer 6 输入：Player
5. 回到Player对象，Layer选择"Player"
```

✅ 玩家创建完成！

---

## 第六步：创建敌人

### 6.1 创建敌人对象

**1. 创建敌人精灵**

```
Hierarchy右键 > 2D Object > Sprites > Square
改名：CorpseThief
```

**2. 设置位置和颜色**

```
【Transform】
Position: X: 5, Y: 0, Z: 0 （在玩家右侧）

【Sprite Renderer】
Color: 红色 (R:255, G:0, B:0)
```

---

### 6.2 添加物理组件（与玩家相同）

```
1. Add Component > Rigidbody2D
   - Body Type: Dynamic
   - Gravity Scale: 0
   - Freeze Rotation Z: ✅

2. Add Component > CircleCollider2D
   - Radius: 0.5
```

---

### 6.3 添加敌人AI脚本

```
Add Component > 搜索：CorpseThief
点击 "Corpse Thief (Script)"

配置：
Max Health: 80
Move Speed: 3.5
Attack Damage: 15
Attack Range: 1.5
Attack Cooldown: 1.2
Damage Type: Physical
Detection Range: 8
Chase Range: 12
Player Layer: 选择 "Player"
```

---

### 6.4 设置敌人Layer

```
1. 选中CorpseThief
2. Inspector顶部 Layer > Add Layer
3. User Layer 7 输入：Enemy
4. 回到敌人，Layer选择 "Enemy"
```

---

### 6.5 创建多个敌人

**复制敌人：**
```
1. 选中CorpseThief
2. 按 Ctrl+D (Windows) 或 Cmd+D (Mac) 复制
3. 修改新敌人的Position：
   - 敌人2: X: 3, Y: 3
   - 敌人3: X: 3, Y: -3
4. 重命名为：CorpseThief_2, CorpseThief_3
```

---

### 6.6 配置物理碰撞

**让玩家和敌人可以互相碰撞：**

```
菜单栏：Edit > Project Settings > Physics 2D

找到最下方的 Layer Collision Matrix（层碰撞矩阵）

设置：
✅ Player 与 Enemy 可以碰撞
✅ Enemy 与 Player 可以碰撞
✅ Enemy 与 Enemy 可以碰撞
```

---

## 第七步：运行游戏！

### 7.1 保存场景和项目

**一定要保存！**
```
File > Save Scene (或按 Ctrl+S)
File > Save Project (或按 Ctrl+Shift+S)
```

---

### 7.2 点击Play按钮

**在Unity顶部中间，有三个按钮：**
```
▶️ Play （播放/运行游戏）
⏸️ Pause （暂停）
⏭️ Step （单步执行）
```

**点击 ▶️ Play 按钮！**

---

### 7.3 测试游戏

**Game视图会变成游戏画面：**

**控制操作：**
```
W A S D - 移动玩家（绿色方块）
Q - 千钧坠（范围攻击）
W - 铜墙铁壁（盾反）
E - 处决（需要敌人低血量）
```

**观察效果：**
```
✅ 玩家可以移动
✅ 按Q可以攻击敌人
✅ 敌人会追击玩家
✅ 敌人会攻击玩家
✅ 按W会看到镜头轻微震动（盾反效果）
```

---

### 7.4 查看Console日志

**打开Console窗口：**
```
Window > General > Console
（或按 Ctrl+Shift+C）
```

**你会看到详细的战斗日志：**
```
玩家生成于 (0, 0, 0)
千钧坠！击中 3 个目标
受到 15 点Physical伤害 (减伤: 25%) | 剩余生命: 985/1000
陶层破裂！剩余 2 层，伤害增幅: +5%
【巨型尸将】重拳！
盾反！反弹 25 点伤害
```

---

### 7.5 停止游戏

**再次点击 ▶️ Play 按钮**
- 按钮会从红色变回灰色
- 游戏停止运行

⚠️ **注意：游戏运行时的所有修改都不会保存！**
- 如果要调整参数，必须先停止游戏
- 然后在Scene视图中修改
- 再点Play测试

---

## 常见问题解决

### ❓ 问题1：点击Play后，玩家不动

**可能原因和解决方法：**

```
1. 检查TerracottaWarrior脚本是否正确挂载
   - 选中Player
   - Inspector中应该看到 "Terracotta Warrior (Script)"
   - 如果显示 "Script is missing"，重新添加脚本

2. 检查Move Speed是否大于0
   - 在TerracottaWarrior组件中
   - Move Speed应该是 5

3. 检查Rigidbody2D设置
   - Body Type应该是 Dynamic
   - Constraints中只勾选 Freeze Rotation Z
```

---

### ❓ 问题2：按Q没有反应

**检查：**

```
1. Damage Layer是否设置正确
   - 选中Player
   - 找到Slam (Q)部分
   - Damage Layer应该包含 "Enemy" 层

2. 敌人是否在范围内
   - Slam Radius默认是3
   - 敌人必须在3米范围内才能被击中

3. 查看Console是否有错误
   - 红色错误信息说明代码有问题
```

---

### ❓ 问题3：敌人不攻击玩家

**检查：**

```
1. Player的Tag是否设置为 "Player"
2. CorpseThief的Player Layer是否设置为 "Player"
3. Detection Range是否足够大（默认8米）
4. 玩家是否在敌人的检测范围内
```

---

### ❓ 问题4：游戏很卡

**优化建议：**

```
1. 关闭不必要的窗口
   - 只保留Scene、Game、Inspector、Hierarchy

2. 降低Game视图的分辨率
   - 点击Game标签
   - 找到上方的分辨率下拉框
   - 选择较小的分辨率（如1280x720）

3. 关闭Scene视图的Gizmos
   - Scene视图右上角
   - 点击 "Gizmos" 按钮关闭

4. 检查电脑配置
   - Unity推荐配置：8GB+ 内存，独立显卡
```

---

### ❓ 问题5：脚本报错

**常见错误和解决：**

```
错误1: "CS0246: The type or namespace name 'XXX' could not be found"
解决：
- File > Save All
- Assets > Reimport All

错误2: "Script is missing"
解决：
- 删除出错的组件
- 重新 Add Component 添加脚本

错误3: "NullReferenceException"
解决：
- 检查Inspector中的字段是否为空
- 确保所有必填字段都已赋值
```

---

### ❓ 问题6：找不到脚本

**如果Add Component时搜索不到脚本：**

```
1. 检查脚本文件是否在正确位置
   - Project窗口
   - Assets/Scripts/ 下应该有所有脚本

2. 重新导入项目
   - 关闭Unity
   - 删除项目根目录的 Library 文件夹
   - 重新打开项目

3. 检查脚本语法错误
   - 打开Console窗口
   - 查看是否有红色错误
   - 修复所有错误后脚本才会出现在组件列表
```

---

### ❓ 问题7：窗口布局乱了

**恢复默认布局：**

```
1. 右上角找到 "Layout" 下拉菜单
2. 选择 "Default"
3. 或者选择 "Revert Factory Settings"（恢复出厂设置）
```

---

## 🎯 下一步学习

### 学习资源推荐

**官方教程：**
```
1. Unity Learn 官方教程
   https://learn.unity.com/
   - 免费
   - 有中文字幕
   - 适合初学者

2. Unity 官方文档
   https://docs.unity3d.com/
   - 查找组件和API用法
```

**视频教程：**
```
1. B站搜索：Unity 2D游戏教程
2. YouTube搜索：Unity 2D Tutorial
3. 推荐UP主：M_Studio, 智能帮, siki学院
```

---

## 🎮 修改游戏的小技巧

### 调整游戏数值

**让游戏更简单：**
```
选中Player，在Inspector中：
- Max Health: 2000 （加倍生命值）
- Move Speed: 8 （移动更快）
- Slam Damage: 600 （伤害加倍）

选中敌人：
- Max Health: 40 （敌人更脆）
- Attack Damage: 5 （攻击更低）
```

**让游戏更难：**
```
选中Player：
- Max Health: 500 （减半生命）
- Clay Layers: 2 （护甲层数减少）

选中敌人：
- Max Health: 150 （敌人更肉）
- Move Speed: 5 （追的更快）
- Attack Damage: 30 （攻击更高）
```

---

### 创建Boss战

**复制尸化盗贼，改成Boss：**

```
1. 复制一个敌人
2. 改名：Boss
3. 删除 CorpseThief 组件
4. Add Component > GiantCorpseGeneral
5. 设置参数：
   - Max Health: 800
   - Mercury Shield Amount: 300
   - 其他参数参考脚本默认值
6. 调整Scale让Boss更大：
   - Scale: X: 2, Y: 2, Z: 1
```

---

### 改变颜色和大小

**让玩家更显眼：**
```
选中Player
Sprite Renderer > Color: 选择更亮的颜色
Transform > Scale: X: 1.5, Y: 1.5 （放大1.5倍）
```

---

## 📝 常用快捷键

```
通用：
Ctrl + S - 保存场景
Ctrl + Shift + S - 保存项目
Ctrl + Z - 撤销
Ctrl + Y - 重做
Ctrl + D - 复制选中对象

场景视图：
F - 聚焦到选中对象
W - 移动工具
E - 旋转工具
R - 缩放工具
Q - 手形工具（平移视角）

运行：
Ctrl + P - 播放/停止游戏
Ctrl + Shift + P - 暂停游戏

窗口：
Ctrl + 1 - Scene视图
Ctrl + 2 - Game视图
Ctrl + Shift + C - Console窗口
```

---

## 🎉 恭喜你！

如果你完成了这个教程，说明你已经：

✅ 学会了Unity基础操作
✅ 创建了第一个可玩的游戏场景
✅ 理解了游戏对象、组件、脚本的关系
✅ 成功运行了秦陵兵马俑ARPG原型

**继续探索：**
- 尝试调整各种数值
- 创建更多敌人
- 学习Unity的动画系统
- 添加音效和特效

**遇到问题？**
- 查看README.md
- 搜索Unity官方文档
- 在Unity论坛提问
- B站搜索相关教程

---

**祝你在Unity的学习之旅中一切顺利！** 🚀✨

*记住：每个大神都是从小白开始的，多练习就会进步！*
