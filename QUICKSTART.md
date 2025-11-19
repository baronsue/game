# 快速开始指南

本指南将帮助你在5分钟内运行起秦陵兵马俑ARPG项目。

---

## 📋 前置要求

1. **Unity Hub** (最新版本)
2. **Unity 2021.3 LTS** 或更高版本
3. **Universal Render Pipeline (URP)** 包
4. **TextMeshPro** 包

---

## 🚀 第一步：打开项目

### 方法1：通过Unity Hub
```
1. 打开Unity Hub
2. 点击"Add" > "Add project from disk"
3. 选择本项目的根目录
4. 点击项目名称打开
```

### 方法2：直接打开
```
1. 启动Unity
2. File > Open Project
3. 选择本项目的根目录
```

---

## 🎮 第二步：创建测试场景

### 1. 新建场景
```
File > New Scene > 2D (URP)
保存为：Assets/Scenes/TestScene.unity
```

### 2. 设置摄像机
```
选中Main Camera
- 投影模式：Orthographic
- Size：5-10（根据需要调整）
- Background：纯黑色
```

### 3. 创建游戏管理器
```
右键Hierarchy > Create Empty
命名为：GameManager
添加组件：GameManager.cs
```

### 4. 创建音频系统
```
右键Hierarchy > Create Empty
命名为：AudioHub
添加组件：AudioHub.cs
```

### 5. 创建镜头震动
```
右键Hierarchy > Create Empty
命名为：CameraShaker
添加组件：CameraShaker.cs
配置：Main Camera = 拖入Main Camera
```

---

## 🤺 第三步：创建玩家角色

### 1. 创建玩家对象
```
右键Hierarchy > 2D Object > Sprite
命名为：Player
```

### 2. 配置玩家
```
【Transform】
Position：(0, 0, 0)

【Sprite Renderer】
Sprite：暂时使用Unity内置的方块精灵
Color：绿色（表示玩家）

【添加组件：Rigidbody2D】
Body Type：Dynamic
Gravity Scale：0
Constraints：Freeze Rotation Z

【添加组件：CircleCollider2D】
Radius：0.5

【添加组件：TerracottaWarrior】
Max Health：1000
Move Speed：5
Slam Radius：3
Slam Damage：300
Shield Duration：1.2
Damage Layer：选择Enemy层

【设置Tag】
Tag：Player
```

---

## 👾 第四步：创建敌人

### 1. 创建敌人对象
```
右键Hierarchy > 2D Object > Sprite
命名为：CorpseThief
```

### 2. 配置敌人
```
【Transform】
Position：(5, 0, 0)

【Sprite Renderer】
Sprite：方块精灵
Color：红色（表示敌人）

【添加组件：Rigidbody2D】
Body Type：Dynamic
Gravity Scale：0
Constraints：Freeze Rotation Z

【添加组件：CircleCollider2D】
Radius：0.5

【添加组件：CorpseThief】
Max Health：80
Attack Damage：15
Detection Range：8
Player Layer：选择Player层

【设置Layer】
Layer：Enemy（如果没有，创建一个）
```

### 3. 创建多个敌人
```
复制CorpseThief
放置在不同位置：(5, 0), (3, 3), (3, -3)
```

---

## 🎯 第五步：配置Physics 2D

### 1. 创建层级（Layers）
```
Edit > Project Settings > Tags and Layers

Layers:
- Layer 6: Player
- Layer 7: Enemy
```

### 2. 配置碰撞矩阵
```
Edit > Project Settings > Physics 2D

Layer Collision Matrix:
- Player 可以与 Enemy 碰撞
- Enemy 可以与 Player 碰撞
- Enemy 可以与 Enemy 碰撞
```

---

## ▶️ 第六步：运行游戏

### 1. 点击Play按钮

### 2. 测试操作
```
WASD：移动玩家
Q：千钧坠（范围攻击）
W：铜墙铁壁（盾反）
E：处决（需要敌人低血量或眩晕）
```

### 3. 观察Console日志
```
应该看到类似的日志：
- 玩家生成于 (0, 0, 0)
- 千钧坠！击中 X 个目标
- 受到 XX 点Physical伤害
- 陶层破裂！剩余 X 层
```

---

## 🎨 第七步（可选）：添加视觉效果

### 1. 添加2D光照
```
右键Hierarchy > Light > 2D > Global Light 2D
命名为：Global Light

选中Player
右键 > Light > 2D > Point Light 2D
命名为：Spirit Light
配置：
- Intensity：5-10
- Color：绿色
- Radius：3-5
```

### 2. 添加粒子效果
```
选中Player
右键 > Effects > Particle System
命名为：ClayChipsVFX
配置简单的粒子发射（陶片碎裂效果）
```

---

## 🐛 故障排除

### 问题1：玩家不移动
```
检查：
- Rigidbody2D的Constraints是否正确
- TerracottaWarrior脚本是否已挂载
- Move Speed是否大于0
```

### 问题2：技能没有效果
```
检查：
- Damage Layer是否正确设置
- 敌人是否在技能范围内
- Console是否有错误信息
```

### 问题3：敌人不攻击
```
检查：
- Player Layer是否正确设置
- Player的Tag是否为"Player"
- Detection Range是否足够大
```

### 问题4：无法处决
```
检查：
- 敌人血量是否低于30%
- 处决范围是否足够
- 敌人是否眩晕
```

---

## 📊 调试技巧

### 1. 启用Gizmos
```
Scene视图中点击Gizmos按钮
可以看到：
- 黄色圈：千钧坠范围
- 红色圈：处决范围
- 蓝色圈：敌人追击范围
```

### 2. 查看Console日志
```
Window > General > Console
勾选"Collapse"和"Error Pause"
```

### 3. 调整数值
```
运行时可以在Inspector中实时调整：
- 玩家血量
- 技能伤害
- 冷却时间
- 移动速度
```

---

## 🎯 下一步

1. **创建Boss战**
   ```
   - 复制敌人对象
   - 替换脚本为GiantCorpseGeneral
   - 调整血量为800
   - 测试Boss技能
   ```

2. **测试铭契系统**
   ```
   - 右键Assets/Data/Inscriptions
   - Create > TerracottaARPG > Inscription
   - 创建测试铭契
   - 通过代码手动添加到玩家
   ```

3. **完善UI**
   ```
   - 创建Canvas
   - 添加GameHUD脚本
   - 显示血量条和能量条
   ```

---

## 📖 更多资源

- **完整文档**：查看 README.md
- **更新日志**：查看 CHANGELOG.md
- **脚本文档**：查看各脚本的注释

---

## 🆘 获取帮助

遇到问题？

1. 检查Console错误信息
2. 查看README.md的"已知问题"部分
3. 检查脚本注释
4. 提交Issue到项目仓库

---

**祝你开发愉快！** 🎮✨
