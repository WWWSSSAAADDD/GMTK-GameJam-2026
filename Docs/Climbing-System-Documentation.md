# 平台攀爬系统 (Parkour & Climbing System) 使用文档

## 系统概览

攀爬系统基于 **Fantacode Studios Parkour & Climbing System**，核心由以下组件构成：

| 组件 | 命名空间 | 职责 |
|------|----------|------|
| `ClimbPoint` | `FS_ParkourSystem` | 场景中挂载在墙体/平台边缘上的攀爬点 |
| `ClimbController` | `FS_ParkourSystem` | 挂在玩家身上，处理全部攀爬逻辑（悬挂、Shimmy、翻越、跳跃等） |
| `ClimbingPointUtils` | `FS_ParkourSystem` | 编辑器工具：在墙体上自动烘焙/清除攀爬点 |
| `EnvironmentScanner` (partial) | `FS_ThirdPerson` | 检测 Ledge 层、障碍物层的射线/BoxCast |
| `ParkourController` | `FS_ParkourSystem` | 管理 `IsHanging` 状态、预测跳跃（Predictive Jump）、Drop 等 |

---

## 1. 环境配置

### 1.1 设置 Layer

在 Unity 的 `Edit > Project Settings > Tags and Layers` 中确保存在：

- **Ledge** 层 — 所有可攀爬的墙体/平台边缘必须设置为这一层

### 1.2 在墙体上烘焙 ClimbPoint

打开 **Tools > Parkour & Climbing System > ClimbPoint Editor**。

#### 操作步骤

1. 选中一个带有 `MeshRenderer` 的墙体 GameObject
2. 设置 **Distance Between Points**（相邻点的间距，默认 0.75 米，最小 0.05）
3. 勾选 **Both Side** 可在墙体两侧同时生成点（如薄墙需要两侧都可抓取）
4. 点击 **Bake** 按钮：
   - 自动将物体设为 Ledge 层
   - 自动在墙体最长的边缘上均匀生成 `ClimbPoint` 子对象
   - 每个攀爬点位置在墙体外侧边缘（根据 bounds 自动计算）
   - `ClimbPoint` 的 forward 方向指向墙体外部（即玩家抓握时面向的方向）
5. 点击 **Clear Points** 清除该物体上所有已生成的攀爬点

#### Bake 逻辑详解

脚本会比较物体的 X/Y/Z 三个维度的长度，在 **最长维度的上边缘** 放置攀爬点。例如：

- 横向长墙（X 最长）→ 在墙体顶部边缘沿 X 轴均匀布点
- 竖直高墙（Y 最长）→ 在墙体顶部沿 Y 轴均匀布点
- 纵向长墙（Z 最长）→ 在墙体顶部边缘沿 Z 轴均匀布点

### 1.3 ClimbPoint 参数

每个 `ClimbPoint` 组件有以下可调参数：

| 参数 | 类型 | 说明 |
|------|------|------|
| `useManualOptions` | bool | 启用手动覆盖选项（启用后可手动设置 handSpacing 和 MountPoint） |
| `MountPoint` | bool | 标记为可翻越点 — 玩家可以从悬挂状态翻越上去的平台 |
| `handSpacing` | float | 手动指定左右手间距（覆盖 ClimbController 的默认 handSpacing） |

**ClimbPoint 的 forward 方向（蓝色箭头 Gizmo）指向墙外**，即玩家悬挂时脸朝向的方向。Bake 工具会自动处理此方向。

### 1.4 手动连接两个 ClimbPoint

选中两个 `ClimbPoint` GameObject，可通过 `ClimbingPointUtils.ConnectTwoPoints()` API 建立连接，支持连接类型：

| 连接类型 | 说明 |
|----------|------|
| `ConnectionType.Move` | 横向移动连接（Shimmy） |
| `ConnectionType.Jump` | 跳跃连接（用于跨 ledge 跳点或对角线移动） |
| `ConnectionType.None` | 无连接 |

---

## 2. 玩家配置

### 2.1 创建攀爬角色

打开 **Tools > Parkour & Climbing System > Create Character**。

流程：

1. 拖入一个人形 FBX 模型
2. 系统会自动实例化 `Parkour Controller` 预制体
3. 将模型挂载到角色层级下（作为 LocomotionController 的子对象）
4. 自动配置 Animator（复制 Avatar）、Camera（设置 Follow Target）、FootTrigger（在左右脚骨骼下创建碰撞体）

### 2.2 必需的玩家组件

| 组件 | 作用 |
|------|------|
| `ClimbController` | 核心攀爬控制器 |
| `ParkourController` | 跑酷控制器（管理 `IsHanging` 状态） |
| `EnvironmentScanner` | 环境检测（Ledge 层、障碍物射线） |
| `ParkourInputManager` | 攀爬专用输入（Jump、Drop、JumpFromHang） |
| `LocomotionInputManager` | 移动输入（用于悬挂时的 Shimmy 方向控制） |
| `CharacterController` | Unity 标准角色控制器 |
| `Animator` | 人形动画控制器（必须为 Humanoid Avatar） |

### 2.3 ClimbController 关键参数

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `enableClimbing` | bool | true | 启用/禁用攀爬功能 |
| `handOffsets` | Vector3 | `(0, -0.08, 0.05)` | 手部相对于攀爬点的偏移量，调整此值确保手部正确对齐 ledge |
| `footPlacementOffset` | float | 0.15 | 脚部 IK 距离墙面的偏移量 |
| `footIkRayLength` | float | 0.5 | 检测脚部 IK 位置的射线长度 |
| `hipRayLength` | float | 0.3 | 检测背后墙壁的射线长度（用于判断 Braced/Free Hang） |
| `pressInputToClimb` | bool | false | 如果勾选，需要按 Jump 键才能向上翻越（而非自动触发） |
| `predictiveBackJumpOnBackwardInput` | bool | true | 悬挂时按后退键自动触发预测后跳 |
| `predictiveBackJumpBackwardThreshold` | float | 0.6 | 触发预测后跳的最小后退输入量（0.1~1） |

### 2.4 ParkourController 关键参数

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `enablePredictiveJump` | bool | true | 启用预测跳跃 |
| `MaxJumpHeight` | float | 1.2 | 最大跳跃高度 |
| `MaxJumpDistance` | float | 7 | 最大跳跃距离 |
| `RotationSpeed` | float | 500 | 攀爬/跑酷动作中的旋转速度 |
| `enableAutoStepUp` | bool | true | 自动走上楼梯/台阶 |
| `autoStepForwardInputThreshold` | float | 0.5 | 自动 step up 的最小前进输入对齐度 |

### 2.5 EnvironmentScanner 关键参数

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `MinJumpDistance` | float | 0.8 | 跳跃到目标的最小距离 |
| `jumpToTheClosestLedge` | bool | false | 预测跳跃时选择最近的 ledge（而非最远的） |
| `alwaysUsePlayerForward` | bool | false | 始终使用角色前方进行预测跳跃（而非镜头方向） |

---

## 3. 攀爬操作说明

### 3.1 悬挂类型

系统支持两种悬挂模式，由 Animator 参数 `freeHang` 控制：

| 类型 | `freeHang` 值 | 条件 | 表现 |
|------|--------------|------|------|
| **Braced Hang**（支撑悬挂） | < 0.5 | 背后有墙壁 | 脚踩在墙上，脚部 IK 激活 |
| **Free Hang**（自由悬挂） | ≥ 0.5 | 背后无墙壁 | 双手悬挂，脚悬空，脚部 IK 禁用 |

系统通过 `CheckWall()` 在攀爬点处进行双向 `SphereCast` 检测后方的墙壁来自动决定悬挂类型。

### 3.2 开始攀爬

**方式 A：空中自动抓取（空中按 Jump）**

- 玩家在空中（`isFalling == true`）时按 **Jump** 键
- 系统以玩家位置 + 上方 1.5 米为中心，进行 `OverlapSphere`（半径 0.4 米）搜索 Ledge 层
- 找到最近的 `ClimbPoint` 后播放"空中抓取悬挂"动画
- 根据是否有墙壁，自动选择 Braced Hang / Free Hang

**方式 B：从平台 Drop 到下方 Ledge**

- 站在平台上按 **Drop** 键
- 系统检测前方下方是否有 Ledge（通过 `DropLedgeCheck` 做 SphereCast）
- 如果有，调用 `DropToLedge()` 抓取下方 ledge 的最近点

**方式 C：预测跳跃抓取（Predictive Jump + Climb）**

- 跑酷跳跃时，`FindPointToJump()` 在抛物线中检测 Ledge 层
- 如果检测到可攀爬的 ledge，设置 `jumpPoint.isClimbable = true`
- 后续通过 `ClimbToPoint()` 或 `DoPredictiveClimb()` 衔接攀爬动作

### 3.3 悬挂状态中的操作

**Shimmy（横向侧移）**

- 使用 **方向键/摇杆** 输入（左右为主）
- 系统在当前位置周围搜索相邻 `ClimbPoint`
- 条件：角度差 < 30° 且距离 < 0.9 米
- 执行 `DoShimmyAction()` → 播放 "ClimbTree" 动画
- IK 辅助让手脚在不同时间点匹配新旧攀爬点

**Climb Jump（攀爬跳跃到邻点）**

- 按住方向键 + 按 **Jump**
- 目标 ledge 距离 ≥ 0.9 米或角度差 ≥ 30° 的邻点视为跳跃目标
- 执行 `DoClimbJumpAction()` → `JumpToLedge()`
- 自动判断横向跳跃（x 参数）还是纵向跳跃（y 参数）

**翻越上平台（Mount）**

- **Braced Hang** 时输入 **上方向 > 0.6**
- 系统检测上方是否有足够的攀上空间（`ObstacleCheck`）
- 如果 `currentPoint.useManualOptions` 为 true，还需要该点标记了 `MountPoint`
- 执行 `MountPoint()` → 播放 "BracedHangClimb" / "FreeHangClimb"
- 完成后自动结束悬挂状态

**向后跳 / 预测后跳（Back Jump）**

- 按 **Drop** 键（无方向输入）→ 单纯松手落下（"JumpFromHang" / "JumpFromFreeHang" 动画）
- 按 **JumpFromHang** 键 + 方向 → `HandlePredictiveBackJump()` 向后跳到其他 ledge
- 向后输入超过 `predictiveBackJumpBackwardThreshold`（默认 0.6）→ 自动预测后跳

**强制上爬（仅 `pressInputToClimb = true` 时）**

- 按 **Jump** + 上方输入（Braced Hang 状态下）→ 尝试向上跳出悬挂

### 3.4 结束攀爬

- **翻越成功** → `IsHanging = false`，恢复正常移动
- **Drop 坠落** → `IsHanging = false`，进入 `isFalling` 状态
- **强制跳出** → 同上，可衔接空中抓取
- **墙跑过渡** → 可与其他 Parkour 系统衔接

### 3.5 输入总结

| 操作 | 输入 | 条件 |
|------|------|------|
| 空中抓取 | Jump | 在空中且附近有 Ledge |
| Drop 抓取下方 | Drop | 站在平台上前方下方有 Ledge |
| Shimmy 左/右 | 方向键左/右 | 悬挂中，有邻点 |
| 攀爬跳跃 | 方向键 + Jump | 悬挂中，有较远的邻点 |
| 翻越上平台 | 方向上 > 0.6 | Braced Hang，有翻越空间 |
| 松手坠落 | Drop（无方向输入） | 悬挂中 |
| 预测后跳 | 方向后 > 0.6 | 悬挂中 |
| 预测后跳（手动） | JumpFromHang + 方向 | 悬挂中 |

---

## 4. 运行时逻辑详解

### 4.1 HandleClimbUpdate() 主循环流程

```
FixedUpdate
│
├─ 不在悬挂、不在 Action 中
│  ├─ 没落地 → 累积 isFallingTimer → isFalling = true
│  ├─ isFalling + 按 Jump → 空中 OverlapSphere 搜索 ClimbPoint
│  │  └─ 找到 → ClimbWhenFalling() → 播放抓取动画
│  └─ 按 Drop（未落地 + 不在 Action）
│     └─ DropLedgeCheck 检测前方下方 Ledge
│        └─ 找到 → DropToLedge()
│
└─ 在悬挂中（!InAction）
   ├─ 计算 BackJumpDir（镜头与角色朝向的偏角）
   ├─ 向后跳触发：
   │  ├─ 有后输入超过阈值 / 按 JumpFromHang / 按 Jump(无方向输入)
   │  └─ → HandlePredictiveBackJump()
   ├─ Drop(无方向输入) → DropFromPoint()
   ├─ 上方向 > 0.6（在 HangIdles 动画状态中）
   │  ├─ ObstacleCheck 检测上方空间
   │  ├─ 如果 useManualOptions → 检查 MountPoint 标记
   │  └─ → MountPoint()
   └─ 方向输入 > 0.5
      ├─ pressInputToClimb + Jump + 上方向 → 强制跳出
      ├─ closestPoint(方向输入) 锁定最近 ClimbPoint
      ├─ 距离 < 0.9m 且 角度差 < 30° → DoShimmyAction()
      └─ 距离 ≥ 0.9m 且 按 Jump → DoClimbJumpAction()
```

### 4.2 closestPoint() 搜索逻辑

1. 将 2D 输入转换为 3D 方向：`transform.right * x + transform.up * y`
2. 在 `currentPoint.position + 方向` 处做 `OverlapSphere`（半径 1 米，Ledge 层）
3. 对搜索到的所有 ClimbPoint 做过滤：

   | 过滤条件 | 说明 |
   |----------|------|
   | `Vector3.Angle(当前点forward, 候选点forward) > 100°` | 朝向不能反 |
   | `Vector3.Angle(当前点forward, 候选点forward) > 45° 且 距离 > 1m` | 大角度只接受近距离 |
   | `角度X < 45°` | 输入方向与目标方向的夹角 |
   | `距离 < 距离阈值（动态缩小）` | 优先最近的点 |
   | `CheckSpaceForClimb(point)` | 候选点有足够的悬挂空间且未被占用 |
   | `!RayCastCheck(当前点, 候选点)` | 两点之间无障碍物阻挡 |

4. 若有多个符合条件的点，取距离最近的那个

### 4.3 CheckSpaceForClimb() 空间检测

```csharp
// 在攀爬点位置做 BoxCast 检测是否有空间
var halfExtends = new Vector3(0.3f, 0.5f, 0.25f);  // Braced Hang
// 或
var halfExtends = new Vector3(0.3f, 1f, 0.25f);    // Free Hang

Physics.CheckBox(
    point.position + point.forward * 0.6f + Vector3.down * 0.65f,
    halfExtends,
    Quaternion.LookRotation(Vector3.right),
    ObstacleLayer
);
```

- 检测区域在攀爬点的前方（forward）偏移 0.6 米处
- 模拟玩家悬挂时占据的空间
- 返回 false 表示空间被占用，该点不可用

### 4.4 CheckWall() 墙壁检测

```csharp
// 在攀爬点左右两侧各做 SphereCast 检测后方墙壁
Vector3 rightFootPoint = point.position
    + (-0.15f) * point.right    // wallRayOffset.x
    + (-0.9f)  * point.up       // wallRayOffset.y
    + 0.14f    * point.forward; // wallRayOffset.z

Physics.SphereCast(rightFootPoint, 0.1f, -point.forward, out hit, hipRayLength, ObstacleLayer);
// 同样检测左脚位置
```

- 对左右脚位置沿 forward 反方向（即墙的方向）做 SphereCast
- 两侧都有命中 → Braced Hang（有墙支撑）
- 任一侧无命中 → Free Hang（自由悬挂）

### 4.5 IK 系统详解

`ClimbController` 使用了复杂的 IK 系统来匹配手脚位置。

#### 初始化阶段

`initializeBodyParts()` 采样动画的快照来获取起始帧和结束帧的手脚位置：

- **startBodyPartOffset**：基于 `previousPoint` 的变换，记录手脚相对于左右手中心点的偏移
- **endBodyPartOffset**：基于 `currentPoint` 的变换，记录动画结束帧的手脚偏移
- 手部偏移额外加上 `handOffsets` 参数

#### 运行时 IK

`OnAnimatorIK()` 中分两种模式：

**精确模式（ikEnabled = true，用于 Shimmy）：**

使用 4 个独立的 lerp 值分别控制右手、左手、右脚、左脚的 IK 过渡时机：

| 身体部位 | lerp 起始 | lerp 结束 | 说明 |
|----------|----------|----------|------|
| 右手 | 0.0 | 0.3 | 先移动 |
| 左手 | 0.5 | 0.8 | 后移动 |
| 右脚 | 0.1 | 0.3 | 与右手相近 |
| 左脚 | 0.6 | 0.9 | 与左手相近 |

如果是镜像动作（mirror），lerp 数组反转。

IK 权重曲线呈山形（中间最高 = 1.3，两端最低），让手在过渡中期最大程度跟随 IK 位置。

**平滑模式（ikEnabled = false，用于其他动作）：**

比较骨骼当前到 `previousVecPoint` 和 `currentVecPoint` 的距离，选择更近的那个 IK 点，权重随距离衰减。

#### MatchTarget

`JumpToLedge()` 和 `DoClimbingAction()` 使用 Unity 的 `Animator.MatchTarget()` 将指定手部（`AvatarTarget.RightHand` 或 `LeftHand`）匹配到攀爬点的 IK 位置。

### 4.6 翻越（Mount）的条件判断

```csharp
var hitData = envScanner.ObstacleCheck(
    forwardOriginOffset: (currentPoint.position.y - transform.position.y) + 0.2f
);

// 条件 1：未使用手动选项 → 自动判断
(hitData.forwardHitFound && hitData.heightHitFound && hitData.hasSpace
    && (hitData.heightHit.point.y - currentPoint.transform.position.y) < 0.2f)

// 条件 2：使用了手动选项 → 需要 MountPoint 标记
(currentPoint.useManualOptions && currentPoint.MountPoint)
```

---

## 5. 快速上手流程

### 步骤 1：场景准备

将可攀爬平台的 GameObject 设为 **Ledge** 层。

### 步骤 2：烘焙攀爬点

`Tools > Parkour & Climbing System > ClimbPoint Editor`

1. 选中平台
2. 设置间距（推荐 0.5~0.75）
3. 点击 **Bake**

### 步骤 3：创建角色

`Tools > Parkour & Climbing System > Create Character`

1. 拖入人形 FBX 模型
2. 系统自动生成完整的攀爬角色

### 步骤 4：确认配置

检查角色上的组件：

- `ClimbController` → `enableClimbing = true`
- `ParkourController` → `enablePredictiveJump = true`
- `EnvironmentScanner` → Ledge 层已添加到 LayerMask 中

### 步骤 5：测试

- 跑向平台边缘并按 **Jump** → 预测跳跃并自动抓取 ledge
- **方向键** → Shimmy 横向移动
- 按 **上** → 翻越上平台
- 按 **Drop** → 松手坠落
- 空中按 **Jump** → 抓下方 ledge

---

## 6. 注意事项

1. **ClimbPoint 方向**：forward 方向必须指向**墙外**（玩家面对的方向）。Bake 工具会自动处理，手动创建的 ClimbPoint 需注意。

2. **Bake 精度**：Bake 工具依赖 `MeshRenderer.bounds`，不规则形状的 mesh 可能需要手动调整点位。

3. **Animator 要求**：角色必须为 **Humanoid** 类型，否则 IK 系统（`OnAnimatorIK` / `MatchTarget`）无法工作。

4. **Layer 自动注册**：`EnvironmentScanner.OnEnable()` 会自动将 Ledge 层添加到 `LedgeLayer` 和 `ObstacleLayer` 的 LayerMask 中，但需确保场景中已配置 Ledge 层。

5. **独占用**：每个 `ClimbPoint` 有 `hasOwner` 属性，被某个玩家占用后其他玩家无法使用该点（通过 `currentPoint` setter 自动管理）。

6. **Free Hang 限制**：Free Hang 时，如果邻点比当前点高超过 0.6 米（近距离）或比当前点高（远距离），不允许 Shimmy 过去。这避免了自由悬挂时难以向上移动的物理限制。

7. **碰撞体要求**：`ClimbController` 不需要 `RequireComponent` 特性，但它依赖同 GameObject 上的多个组件（`ICharacter`、`ParkourInputManager`、`LocomotionInputManager`、`ParkourController`、`EnvironmentScanner`），统一挂载在 Player 根对象上。

8. **动画依赖**：攀爬系统依赖以下动画片段（通过字符串名称引用）：

   | 动画名 | 用途 |
   |--------|------|
   | `PredictiveToBracedhang` | 空中抓取到支撑悬挂 |
   | `PredictiveToFreehangOneHanded` | 空中抓取到自由悬挂 |
   | `HangIdles` | 悬挂待机状态 |
   | `ClimbTree` | Shimmy / 攀爬跳跃过渡 |
   | `JumpFromHang` | 支撑悬挂松手跳落 |
   | `JumpFromFreeHang` | 自由悬挂松手跳落 |
   | `BracedHangClimb` | 支撑悬挂翻越上平台 |
   | `FreeHangClimb` | 自由悬挂翻越上平台 |
   | `DropToHang` | 站立状态 Drop 到支撑悬挂 |
   | `DropToFreeHang` | 站立状态 Drop 到自由悬挂 |
   | `IdleToBracedHang` | 站立状态近距离抓取支撑悬挂 |
   | `IdleToFreeHang` | 站立状态近距离抓取自由悬挂 |
   | `FallTree` | 坠落状态 |
