# 秦陵兵马俑 ARPG - Terracotta Warriors ARPG

> 融合秦陵兵马俑文化与暗黑类ARPG玩法的2.5D动作游戏
> 既有《黑神话：悟空》的文化厚度，又有《Hades》的爽快玩法

---

## 🎮 游戏概述

这是一款将中国秦陵兵马俑文化与现代ARPG游戏机制完美结合的2.5D动作游戏。玩家扮演被灵火唤醒的陶俑战士，探索神秘的秦陵地宫，与尸化盗贼、水银怪物和机关巨像战斗。

### 核心特色

- **陶土护甲系统**：陶层破损会增加伤害但降低防御，形成高风险高回报的战斗节奏
- **灵火外泄机制**：血量越低，发光越亮，伤害越高
- **铭契系统**：类似Hades的Boon系统，每关结束选择增益效果
- **处决机制**：对眩晕或低血量敌人施展华丽处决，获得额外奖励
- **技能组合**：千钧坠（Q）控场、铜墙铁壁（W）反弹、处决（E）终结

---

## 📁 项目结构

```
game/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/                 # 核心系统
│   │   │   └── DamagePacket.cs   # 伤害包和接口定义
│   │   ├── Character/            # 角色相关
│   │   │   └── TerracottaWarrior.cs  # 陶俑战士（玩家角色）
│   │   ├── Enemy/                # 敌人AI
│   │   │   ├── EnemyBase.cs      # 敌人基类
│   │   │   ├── CorpseThief.cs    # 尸化盗贼
│   │   │   └── GiantCorpseGeneral.cs  # 巨型尸将Boss
│   │   ├── Systems/              # 游戏系统
│   │   │   ├── AudioHub.cs       # 音频系统
│   │   │   ├── CameraShaker.cs   # 镜头震动
│   │   │   ├── StatusHost.cs     # 状态效果（眩晕、减速）
│   │   │   ├── KnockbackReceiver.cs  # 击退接收
│   │   │   ├── InscriptionManager.cs # 铭契管理
│   │   │   └── GameManager.cs    # 游戏管理器
│   │   ├── Data/                 # 数据定义
│   │   │   └── Inscription.cs    # 铭契ScriptableObject
│   │   └── UI/                   # 用户界面
│   │       ├── GameHUD.cs        # 游戏HUD
│   │       ├── ExecutePromptUI.cs    # 处决提示UI
│   │       └── InscriptionSelectionUI.cs  # 铭契选择UI
│   ├── Data/
│   │   └── Inscriptions/         # 铭契数据文件
│   ├── Scenes/                   # 场景文件
│   ├── Prefabs/                  # 预制体
│   ├── Sprites/                  # 2D精灵
│   ├── Materials/                # 材质
│   └── Audio/                    # 音频资源
└── README.md
```

---

## 🛠️ 技术栈

- **引擎**：Unity 2021.3+ (LTS)
- **渲染**：Universal Render Pipeline (URP) 2D Renderer
- **物理**：Unity 2D Physics
- **语言**：C# (.NET Standard 2.1)
- **美术风格**：2D渲染3D（预渲染序列帧 + 法线贴图 + 实时光照）

---

## 🎯 核心系统说明

### 1. 伤害系统 (DamagePacket)

```csharp
public enum DamageType { Physical, Mercury, Fire, Spirit, Corrosion }

public struct DamagePacket {
    public float amount;        // 伤害数值
    public DamageType type;     // 伤害类型
    public bool isHeavy;        // 是否为重击
    public bool isCritical;     // 是否暴击
    public Transform source;    // 伤害来源
}
```

- **多重减伤计算**：陶层减伤 × 技能减伤 × 抗性（乘法叠加）
- **暴击系统**：15%基础暴击率，2倍暴击倍率
- **五种伤害类型**：物理、水银、火焰、灵力、腐蚀

### 2. 陶土护甲系统

- **陶层机制**：3层陶土护甲，每层提供8.33%减伤（总计25%）
- **破损效果**：每破一层增加5%伤害
- **视觉反馈**：裂纹发光强度随血量降低而增强

### 3. 技能系统

#### Q - 千钧坠
- **范围**：3米半径
- **伤害**：300基础伤害
- **效果**：击退 + 1.5秒减速40%
- **冷却**：3秒

#### W - 铜墙铁壁
- **持续**：1.2秒
- **减伤**：40%
- **反弹**：50%伤害反震
- **特殊**：反弹重击附带1.5秒眩晕
- **冷却**：5秒

#### E - 处决
- **范围**：2米
- **条件**：目标血量<30% 或 眩晕状态
- **伤害**：200基础 + 目标当前生命20%
- **奖励**：恢复20点灵火能量

### 4. 铭契系统

类似Hades的Boon系统，关卡结束后2选1：

| 铭契名称 | 稀有度 | 效果 |
|---------|--------|------|
| 饕餮·贪噬 | 史诗 | 处决后3秒内普攻附1%当前生命上限震裂伤害（叠3） |
| 云雷·坚壁 | 稀有 | 盾墙受击叠加裂纹层（+3%伤害，-2%减伤，6s，叠5） |
| 玄武·定山 | 史诗 | 千钧坠半径+20%，对击退目标必暴击 |
| 魑魅·散毒 | 稀有 | 对汞蚀敌人+25%伤害，水银DoT免疫50% |

---

## 🎨 美术资产生成流程

### A. 建模/贴图
1. **ZBrush**：雕刻高模（陶土崩裂、缺角、刀痕）
2. **Substance Painter**：导出贴图
   - BaseColor（基础颜色）
   - Normal（法线）
   - Roughness（粗糙度）
   - Metallic（金属度）
   - AO（环境光遮蔽）
   - Emissive（裂纹发光遮罩）

### B. 渲染 (Blender)
- **摄像机**：正交，等轴45°俯视（Z旋转45°，X倾斜30°）
- **方向**：8方向（或4向镜像）
- **帧率**：12-16 FPS
- **分辨率**：4K → 下采样至1024-2048px高度

### C. Unity导入
1. **Sprite-Lit材质**：绑定Beauty贴图 + Normal贴图
2. **裂纹发光层**：Additive混合的子SpriteRenderer
3. **视差分层**：前景0.9 / 中景0.6 / 背景0.3

---

## 🗺️ 关卡设计

| 关卡 | 名称 | 视觉风格 | 核心机制 | Boss |
|------|------|----------|----------|------|
| L1 | 兵马俑坑 | 黄土尘束光 | 可推倒土墙压敌 | 巨型尸将 |
| L2 | 水银天河 | 银光冷雾 | 动态液位（20s涨/10s退） | 机关铜蛇 |
| L3 | 青铜地阙 | 青铜绿×金 | 机关齿轮/喷焰/地刺 | 徐福幻影 |

---

## 🚀 快速开始

### 环境要求
- Unity 2021.3 LTS 或更高版本
- Universal Render Pipeline (URP) 已配置
- TextMeshPro 已导入

### 开发测试

1. **创建测试场景**
   ```
   - 新建场景
   - 创建空GameObject，挂载GameManager
   - 创建空GameObject，挂载AudioHub
   - 创建空GameObject，挂载CameraShaker
   - 主摄像机添加URP 2D Renderer
   ```

2. **创建玩家角色**
   ```
   - 创建2D Sprite对象
   - 添加Rigidbody2D（Gravity Scale = 0）
   - 添加CircleCollider2D
   - 挂载TerracottaWarrior脚本
   - 设置Tag为"Player"
   - 配置技能参数（千钧坠、铜墙铁壁）
   ```

3. **创建敌人**
   ```
   - 创建2D Sprite对象
   - 添加Rigidbody2D（Gravity Scale = 0）
   - 添加CircleCollider2D
   - 挂载CorpseThief或GiantCorpseGeneral脚本
   - 设置Layer为敌人层
   ```

4. **测试战斗**
   ```
   - 运行场景
   - WASD移动
   - Q施放千钧坠
   - W激活铜墙铁壁
   - E处决敌人
   - 鼠标左键普通攻击
   ```

### 调试模式

游戏提供详细的Console日志输出：

```csharp
// 战斗日志示例
受到 35 点Physical伤害 (减伤: 25%) | 剩余生命: 965/1000
陶层破裂！剩余 2 层，伤害增幅: +5%
千钧坠！击中 3 个目标
盾反！反弹 25 点伤害
重击被反震！眩晕 1.5s
处决成功！造成 250 点伤害
```

---

## 🎮 操作说明

| 按键 | 功能 |
|------|------|
| WASD | 移动 |
| Q | 千钧坠（范围AOE + 击退） |
| W | 铜墙铁壁（盾反 + 眩晕） |
| E | 处决（终结技） |
| 鼠标左键 | 普通攻击 |
| 空格 | 闪避（待实现） |
| ESC | 暂停菜单 |

---

## 📊 数值设计

### 玩家（陶俑战士）
- **生命值**：1000
- **陶层**：3层（每层8.33%减伤）
- **移动速度**：5 m/s
- **暴击率**：15%
- **暴击倍率**：2.0x

### 敌人

#### 尸化盗贼
- **生命值**：80
- **伤害**：15
- **移动速度**：3.5 m/s
- **特性**：冲锋（距离5m时触发）

#### 巨型尸将（Boss）
- **生命值**：800
- **水银护盾**：300
- **阶段1**：重拳（50伤害）、骨链横扫（35伤害，4m范围）
- **阶段2**（30%血以下）：水银脉冲（25伤害，8m范围，7s间隔）

---

## 🔧 扩展开发

### 添加新铭契

1. 在Unity中右键 `Assets/Data/Inscriptions`
2. 选择 `Create > TerracottaARPG > Inscription`
3. 填写属性（ID、名称、描述、效果类型、数值）
4. 在 `Inscription.cs` 的 `ApplyToWarrior` 方法中实现效果逻辑

### 添加新敌人

1. 创建新脚本继承自 `EnemyBase`
2. 重写以下方法：
   ```csharp
   protected override void Awake() { }
   protected override void PerformAttack() { }
   protected override void Die() { }
   ```
3. 配置敌人属性（血量、伤害、移动速度）

### 添加新技能

在 `TerracottaWarrior.cs` 中：

1. 定义技能参数
2. 在 `HandleInput` 中添加按键监听
3. 实现技能逻辑（参考 `DoSlam` 和 `ShieldWallCO`）
4. 添加冷却管理

---

## 📝 待实现功能

- [ ] 完整的角色动画系统
- [ ] 粒子特效（地裂、盾反、水银等）
- [ ] 完整的音效库（FMOD/Wwise集成）
- [ ] 存档系统
- [ ] 更多铭契种类（目标：30+）
- [ ] 装备系统（武器、防具）
- [ ] 关卡2和关卡3的完整实现
- [ ] 主菜单和角色选择界面
- [ ] 成就系统
- [ ] 难度选择

---

## 🐛 已知问题

- 音频系统目前仅输出日志，需要实际音频资源
- 镜头震动需要Cinemachine支持才能达到最佳效果
- UI预制体需要手动创建（InscriptionSelectionUI的按钮预制体）
- 处决动画为占位逻辑，需要Timeline或Animator实现

---

## 📖 设计参考

### 游戏参考
- **《Hades》**：铭契系统、关卡结构、流畅战斗
- **《暗黑破坏神3》**：装备系统、数值设计
- **《黑神话：悟空》**：文化表达、视觉风格
- **《死亡细胞》**：战斗节奏、技能组合

### 文化参考
- 秦始皇陵兵马俑博物馆
- 《史记·秦始皇本纪》
- 中国古代青铜器纹饰
- 道教五行相生相克理论

---

## 🤝 贡献指南

欢迎提交Issue和Pull Request！

1. Fork本项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启Pull Request

---

## 📄 许可证

本项目仅用于学习和演示目的。

---

## 👥 致谢

- Unity Technologies - 游戏引擎
- Supergiant Games - Hades游戏设计灵感
- 中国秦始皇陵博物院 - 文化素材参考

---

## 📧 联系方式

项目维护者：[Your Name]
邮箱：[your.email@example.com]
Discord：[Your Discord]

---

**祝你在秦陵地宫中战斗愉快！愿灵火永不熄灭！** 🔥⚔️
