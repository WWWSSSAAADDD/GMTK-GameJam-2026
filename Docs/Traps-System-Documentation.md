# 陷阱系统 (Traps) 脚本使用文档

## 目录

1. [DebugReturnToSafety — 调试返回安全点](#1-debugreturntosafety--调试返回安全点)
2. [TrapDetectionZoneGizmo — 检测区域可视化](#2-trapdetectionzonegizmo--检测区域可视化)
3. [MovingPlatformTrap — 移动平台陷阱](#3-movingplatformtrap--移动平台陷阱)
4. [MemePusherTrap — 迷因推动陷阱](#4-memepushertrap--迷因推动陷阱)
5. [CageDropTrap — 笼子掉落陷阱](#5-cagedroptrap--笼子掉落陷阱)
6. [DirectionalWallTrap — 方向感应墙陷阱](#6-directionalwalltrap--方向感应墙陷阱)
7. [DelayedImpulseTrap — 延时冲量陷阱](#7-delayedimpulsetrap--延时冲量陷阱)
8. [DisappearingBlockTrap — 消失方块陷阱](#8-disappearingblocktrap--消失方块陷阱)
9. [AppearingObstacleTrap — 出现障碍物陷阱](#9-appearingobstacletrap--出现障碍物陷阱)
10. [通用注意事项](#通用注意事项)

---

## 1. DebugReturnToSafety — 调试返回安全点

**路径:** `Assets\Scripts\Trap\DebugReturnToSafety.cs`
**命名空间:** `CountdownTraps`

### 用途
仅在 **编辑器模式**（`#if UNITY_EDITOR`）下生效。提供调试热键让玩家瞬间回到初始位置，方便关卡设计师反复测试陷阱。

### 使用方式
1. 将此脚本挂载到带有 `CharacterController` 的玩家对象上
2. 运行时按下 **主键盘 1** 或 **小键盘 1**
3. 玩家立即传送回 `Awake()` 时的位置和朝向
4. 脚本通过临时禁用再启用 `CharacterController` 来避免物理碰撞干扰传送

### 公开参数
无。

---

## 2. TrapDetectionZoneGizmo — 检测区域可视化

**路径:** `Assets\Scripts\Trap\TrapDetectionZoneGizmo.cs`
**命名空间:** `CountdownTraps`

### 用途
纯编辑器辅助脚本，在 Scene 视图中用**半透明方块**绘制陷阱的触发区域，方便关卡设计师直观观察各陷阱的检测范围。不影响运行时逻辑。

### 使用方式
1. 挂载到任何带有 `BoxCollider`（`isTrigger = true`）的陷阱对象上
2. `detectionZone` 字段可手动指定要绘制的 BoxCollider
3. 若留空，脚本会自动查找对象上第一个 `isTrigger = true` 的 BoxCollider
4. 选中对象后在 Scene 视图中即可看到半透明彩色方块

### 参数

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `detectionZone` | BoxCollider | null | 要显示的区域碰撞体引用，留空自动查找 |
| `gizmoColor` | Color | `(1, 0.15, 0.05, 0.16)` | Gizmo 颜色，默认半透明橙红色 |

### 显示效果
- 实心半透明方块：填充区域
- 不透明线框：边界轮廓

---

## 3. MovingPlatformTrap — 移动平台陷阱

**路径:** `Assets\Scripts\Trap\MovingTrap\MovingPlatformTrap.cs`
**命名空间:** `CountdownTraps`

### 用途
玩家进入检测区域后，整个平台沿世界坐标方向平滑移动，把玩家带向危险区域（如深渊或陷阱中心）。**仅触发一次，不重置。**

### 使用方式
1. 创建一个包含**两个 BoxCollider** 的游戏对象：
   - **第一个 BoxCollider**：平台物理碰撞体（玩家可站立，非 Trigger）
   - **第二个 BoxCollider**：检测区域（`isTrigger = true`）
2. 挂载此脚本
3. `Reset()` 会自动将第二个 BoxCollider 赋值给 `detectionZone`
4. 设置 `moveOffset`（移动位移）和 `moveDuration`（持续时间）
5. 推荐同时添加 `TrapDetectionZoneGizmo` 以可视化检测范围

### 参数

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `moveOffset` | Vector3 | `(0, 0, 8)` | 平台移动的世界空间位移量 |
| `moveDuration` | float | 0.3 | 移动持续时间（秒），最小值 0.05 |
| `detectionZone` | BoxCollider | null | 检测区域碰撞体引用 |
| `detectionCenter` | Vector3 | `(0, 0, 0)` | 检测区域中心偏移 |
| `detectionSize` | Vector3 | `(1, 2, 2)` | 检测区域尺寸 |

### 行为流程
1. 玩家进入检测区域 → `triggered = true`
2. 平台从当前位置在 `moveDuration` 秒内 **Lerp** 到 `当前坐标 + moveOffset`
3. 移动完成后静止，不可再次触发

### 玩家识别逻辑
- 碰撞体 Tag 为 `"Player"`
- 或碰撞体及其父对象上存在 `CharacterController` 组件

---

## 4. MemePusherTrap — 迷因推动陷阱

**路径:** `Assets\Scripts\Trap\MemeTrap\MemePusherTrap.cs`
**命名空间:** `CountdownTraps`

### 用途
玩家进入检测区域后，一个**迷因（meme）物体**激活并沿其自身 `forward` 方向滑动。如果迷因的碰撞体接触到玩家，会将玩家一起推动，可用于将玩家推下平台。**仅触发一次。**

### 使用方式
1. 创建一个带有 BoxCollider（`isTrigger = true`）的检测区域游戏对象
2. 挂载此脚本，指定 `detectionZone`
3. 在场景中放置一个 **meme 物体**（任意带非 Trigger Collider 的 3D 模型），引用到 `meme` 字段
   - 运行时 meme 物体会自动隐藏（`Awake` 中 `SetActive(false)`）
   - meme 的 **自身 forward 方向** 决定其移动方向
   - meme 上需要带有**至少一个非 Trigger 的 Collider** 用于与玩家进行碰撞检测

### 参数

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `meme` | GameObject | null | 要移动的迷因物体引用 |
| `memeMoveSpeed` | float | 12 | 迷因移动速度，最小值 0.1 |
| `memeTravelDistance` | float | 10 | 迷因移动距离，最小值 0.1 |
| `detectionZone` | BoxCollider | null | 检测区域碰撞体 |
| `detectionCenter` | Vector3 | `(0, 0, 0)` | 检测区域中心偏移 |
| `detectionSize` | Vector3 | `(1, 2, 2)` | 检测区域尺寸 |

### 行为流程
1. 玩家进入检测区域 → `triggered = true`
2. meme 物体激活（`SetActive(true)`）
3. 计算目标位置：`meme当前位置 + forward * memeTravelDistance`
4. 每帧执行：
   - meme 沿 forward 方向以 `memeMoveSpeed` 速度移动
   - 检测 meme 所有子 Collider 是否与玩家 `CharacterController.bounds` 相交
   - 若接触，通过 `playerController.Move(memeMoveDelta)` 推动玩家同步位移
5. meme 到达目标位置后停止

### 玩家识别逻辑
- 碰撞体或其父对象上有 `CharacterController`
- 且碰撞体 Tag 或 `CharacterController` Tag 为 `"Player"`

---

## 5. CageDropTrap — 笼子掉落陷阱

**路径:** `Assets\Scripts\Trap\CageTrap\CageDropTrap.cs`
**命名空间:** `CountdownTraps`

### 用途
玩家进入触发区域后，笼子的**四面墙 + 屋顶**立即出现将玩家困住。短暂延迟后，**地板消失**，玩家坠落。**仅触发一次。**

### 使用方式
1. 创建一个带有 Collider（`isTrigger = true`）的触发区域游戏对象
2. 在场景中准备笼子各部件作为独立 GameObject：
   - 西墙 `cageWest`
   - 东墙 `cageEast`
   - 南墙 `cageSouth`
   - 北墙 `cageNorth`
   - 屋顶 `cageRoof`
   - 可移除地板 `floorToRemove`
3. 将所有引用拖入脚本参数槽
4. **注意：** 笼子部件在场景中初始应为**可见**（用于编辑定位），脚本 `Awake()` 时自动隐藏

### 参数

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `floorToRemove` | GameObject | null | 触发后要移除的地板 |
| `cageWest` | GameObject | null | 笼子西墙 |
| `cageEast` | GameObject | null | 笼子东墙 |
| `cageSouth` | GameObject | null | 笼子南墙 |
| `cageNorth` | GameObject | null | 笼子北墙 |
| `cageRoof` | GameObject | null | 笼子屋顶 |
| `dropDelay` | float | 0.25 | 笼子出现到地板消失的延迟（秒），最小值 0 |

### 行为流程
1. 玩家进入触发区域 → `triggered = true`
2. 笼子四面墙 + 屋顶立即激活（`SetActive(true)`），困住玩家
3. 等待 `dropDelay` 秒后
4. `floorToRemove` 禁用（`SetActive(false)`），玩家坠落
5. 陷阱永久生效，不重置

---

## 6. DirectionalWallTrap — 方向感应墙陷阱

**路径:** `Assets\Scripts\Trap\CageTrap\DirectionalWallTrap.cs`
**命名空间:** `CountdownTraps`

### 用途
**最复杂的陷阱。** 玩家在检测区域内移动时，系统根据**玩家的移动输入方向**逐步升起对应方向的墙壁。当四面墙壁全部升起后，地板掉落。玩家离开区域后，**延迟自动重置**整个陷阱，使其可重复使用。

### 核心机制

- **渐进式困住玩家：** 玩家越移动，墙壁越多
- **方向感应：** 每面墙对应不同的移动输入方向
- **四面归位后触发：** 四面墙全升起 → 地板掉落
- **可重置：** 玩家离开后延迟恢复

### 使用方式
1. 创建一个带有 BoxCollider（`isTrigger = true`）的检测区域游戏对象
2. 在场景中放置四面墙（forward/right/back/left）和一块地板
3. 将所有 GameObject 引用拖入脚本参数
4. 墙壁和地板的初始状态由 `Awake()` 中 `ResetTrap()` 设置（地板激活，墙壁隐藏）
5. **关键：根据场景的朝向正确设置四个 `MovementDirection` 参数**
6. 此脚本依赖 `FS_ThirdPerson.LocomotionInputManager` 组件读取玩家输入

### 参数

#### 墙壁与地板引用

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `forwardWall` | GameObject | null | 前方墙壁 |
| `rightWall` | GameObject | null | 右方墙壁 |
| `backWall` | GameObject | null | 后方墙壁 |
| `leftWall` | GameObject | null | 左方墙壁 |
| `floorPlatform` | GameObject | null | 地板平台（四面墙升起后掉落） |

#### 重置设置

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `resetDelay` | float | 2 | 玩家离开后的重置延迟（秒），最小值 0 |

#### 输入检测设置

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `forwardMovementDirection` | Vector3 | `(1, 0, 0)` — 右 | 触发前方墙壁的世界方向 |
| `rightMovementDirection` | Vector3 | `(0, 0, -1)` — 后 | 触发右方墙壁的世界方向 |
| `backMovementDirection` | Vector3 | `(-1, 0, 0)` — 左 | 触发后方墙壁的世界方向 |
| `leftMovementDirection` | Vector3 | `(0, 0, 1)` — 前 | 触发左方墙壁的世界方向 |
| `directionDotThreshold` | float | 0.7 | 方向匹配阈值（0~1），输入方向与墙壁方向点积需超过此值 |
| `inputThreshold` | float | 0.15 | 输入大小阈值（0.01~1），玩家输入低于此值忽略 |

### 行为流程

#### 触发阶段（每帧 Update）
1. 读取 `LocomotionInputManager.DirectionInput`（玩家移动输入的 2D 向量）
2. 输入大小 < `inputThreshold` → 忽略（防止摇杆漂移误触发）
3. 将 2D 输入转换为 3D 世界方向：`playerTransform.right * x + playerTransform.forward * y`
4. 计算该方向与四个 `MovementDirection` 的**点积**
5. 取**最高点积**的方向，若超过 `directionDotThreshold`，升起对应的墙壁
6. 当四面墙**全部升起** → 立即调用 `DropFloor()`

#### 地板掉落
- 设置 `floorDropped = true`
- 禁用 `floorPlatform` 的 GameObject

#### 重置阶段
1. 玩家离开检测区域 → `OnTriggerExit` 触发
2. 若地板已掉落：启动协程等待 `resetDelay` 秒
3. `ResetTrap()` 执行：
   - 四面墙壁隐藏
   - 地板恢复
   - 所有状态标志重置
4. 若在延迟期间玩家重新进入，**不会中断重置**（当前逻辑下重置照常进行，但新进入会覆盖 `playerInside` 状态）

### 方向配置说明

默认值的设计逻辑（以玩家视角为准）：

| 墙壁名称 | 触发条件（玩家输入） | 默认世界方向 |
|----------|---------------------|--------------|
| `forwardWall`（前墙） | 玩家按 **右** | `(1, 0, 0)` |
| `rightWall`（右墙） | 玩家按 **后** | `(0, 0, -1)` |
| `backWall`（后墙） | 玩家按 **左** | `(-1, 0, 0)` |
| `leftWall`（左墙） | 玩家按 **前** | `(0, 0, 1)` |

可根据关卡实际朝向修改这四个方向向量。

### 玩家识别逻辑
- 碰撞体或其父对象上有 `CharacterController`
- 且碰撞体 Tag 或 `CharacterController` 的 transform Tag 为 `"Player"`
- 进入后缓存 `LocomotionInputManager` 引用用于读取输入

---

## 7. DelayedImpulseTrap — 延时冲量陷阱

**路径:** `Assets\Scripts\Trap\ImpulseTrap\DelayedImpulseTrap.cs`
**命名空间:** `CountdownTraps`

### 用途
玩家进入检测区域后，经过一段**延迟时间**，在**效果区域内**对玩家施加一个**定向冲量**（持续的力），可将玩家推飞。支持一次性触发和重复触发两种模式。

### 使用方式
1. 创建一个带有 BoxCollider（`isTrigger = true`）的检测区域游戏对象
2. 挂载此脚本，指定 `detectionZone`
3. 配置**效果区域**（`effectCenter`、`effectSize`）—— 玩家必须在此区域内才会被施加冲量
4. 设置**冲量方向**（`impulseDirection`，本地坐标）—— 旋转陷阱对象即可旋转冲量方向
5. 可选：拖入 `activationVisual` 用于激活时的视觉反馈
6. 可选：设置 `activationAnimationPoint` 标记激活动画的位置

### 参数

#### 延时

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `activationDelay` | float | 0.8 | 触发后到施加冲量的延迟时间（秒），最小值 0 |

#### 效果区域

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `effectCenter` | Vector3 | `(0, 1.2, 2)` | 效果区域的中心位置（本地坐标） |
| `effectSize` | Vector3 | `(4, 3, 4)` | 效果区域的尺寸 |

#### 冲量

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `impulseDirection` | Vector3 | `(0, 0.65, 1)` | 冲量方向（本地坐标），旋转陷阱可旋转此方向 |
| `impulseSpeed` | float | 14 | 冲量初始速度，最小值 0 |
| `impulseDuration` | float | 0.35 | 冲量持续时间（秒），最小值 0.02 |
| `impulseDebugArrowLength` | float | 4 | 调试箭头的显示长度，仅 Gizmo，最小值 0.1 |

#### 激活反馈

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `activationVisual` | GameObject | null | 可选的可视反馈物体 |
| `hideActivationVisualOnStart` | bool | true | 启动时隐藏反馈物体 |
| `activationAnimationPoint` | Transform | null | 激活动画的标记位置（Gizmo 显示为品红色） |

#### 触发模式

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `triggerOnce` | bool | true | 是否只触发一次。设为 false 则玩家离开后可自动重置重新触发 |
| `repeatResetDelay` | float | 2 | 重复触发模式下的重置延迟（秒），最小值 0 |

#### 检测区域

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `detectionZone` | BoxCollider | null | 检测区域碰撞体 |
| `detectionCenter` | Vector3 | `(0, 0, 0)` | 检测区域中心偏移 |
| `detectionSize` | Vector3 | `(3, 2, 2)` | 检测区域尺寸 |

### 行为流程

1. 玩家进入检测区域 → `triggered = true`
2. 执行 `PlayActivationFeedback()`（可扩展添加音效/动画），激活 visual 显示
3. 等待 `activationDelay` 秒
4. 检查玩家是否在**效果区域**内（`IsPlayerInsideEffectZone`，使用 `OverlapBox` 检测）
5. 若在效果区域内 → 施加冲量：
   - 在 `impulseDuration` 秒内，每帧通过 `CharacterController.Move()` 推动玩家
   - 推力 = 冲量方向 × `impulseSpeed` × 剩余强度 × `Time.deltaTime`
   - 剩余强度从 1.0 线性衰减到 0
6. 冲量完成后标记 `activationComplete = true`
7. 若 `triggerOnce = false` 且玩家已离开检测区域 → 等待 `repeatResetDelay` 秒后重置

### Gizmo 可视化

脚本自带丰富的 Gizmo 绘制：
- **橙红色半透明方块**：检测区域
- **橙色半透明方块**：效果区域
- **橙色箭头**：冲量方向和强度
- **品红色球体**：激活动画标记点

### 玩家识别逻辑
- 碰撞体或其父对象上有 `CharacterController`
- 且碰撞体 Tag 或 `CharacterController` Tag 为 `"Player"`
- 进入时缓存 `trackedPlayer` 引用，离开时清除

---

## 8. DisappearingBlockTrap — 消失方块陷阱

**路径:** `Assets\Scripts\Trap\DisappearingTrap\DisappearingBlockTrap.cs`
**命名空间:** `CountdownTraps`

### 用途
玩家进入检测区域后，经过短暂延迟，一个方块（平台/地板）**消失**，玩家可能因此坠落。支持一次性触发或重复触发（玩家离开后自动恢复）。

### 使用方式
1. 创建一个带有 BoxCollider（`isTrigger = true`）的检测区域游戏对象
2. 在场景中放置一个**可消失的方块/平台**（如地板），拖入 `disappearingBlock`
3. 方块初始状态为**显示**（`Awake` 中确保 `SetActive(true)`）
4. 设置消失延迟 `disappearDelay`

### 参数

#### 消失方块

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `disappearingBlock` | GameObject | null | 要消失的方块/平台引用 |
| `disappearDelay` | float | 0.15 | 触发后到方块消失的延迟（秒），最小值 0 |

#### 触发模式

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `triggerOnce` | bool | true | 是否只触发一次。设为 false 时方块在玩家离开后自动恢复 |
| `repeatResetDelay` | float | 2 | 重复触发模式下的重置延迟（秒），最小值 0 |

#### 检测区域

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `detectionZone` | BoxCollider | null | 检测区域碰撞体 |
| `detectionCenter` | Vector3 | `(0, 0, 0)` | 检测区域中心偏移 |
| `detectionSize` | Vector3 | `(3, 2, 3)` | 检测区域尺寸 |

### 行为流程

1. 玩家进入检测区域 → `triggered = true`，取消待处理的重置
2. 等待 `disappearDelay` 秒
3. `disappearingBlock.SetActive(false)` — 方块消失
4. `blockHidden = true`

**重复触发模式（`triggerOnce = false`）：**

5. 玩家离开检测区域 → 启动重置协程
6. 等待 `blockHidden == true` 且玩家不在区域内后
7. 再等待 `repeatResetDelay` 秒
8. 若玩家仍未进入 → 方块重新出现（`SetActive(true)`）
9. 状态全部重置，陷阱可再次触发

### 玩家识别逻辑
- 碰撞体 Tag 为 `"Player"`
- 或碰撞体及其父对象上存在 `CharacterController` 组件

---

## 9. AppearingObstacleTrap — 出现障碍物陷阱

**路径:** `Assets\Scripts\Trap\AppearingTrap\AppearingObstacleTrap.cs`
**命名空间:** `CountdownTraps`

### 用途
`DisappearingBlockTrap` 的**对称版本**。玩家进入检测区域后，一个隐藏的障碍物**出现**挡住去路。支持一次性触发或重复触发（玩家离开后自动隐藏）。

### 使用方式
1. 创建一个带有 BoxCollider（`isTrigger = true`）的检测区域游戏对象
2. 在场景中放置一个**障碍物**（如尖刺、墙壁），拖入 `appearingObstacle`
3. 障碍物初始状态**自动隐藏**（`Awake` 中 `SetActive(false)`）
4. 设置出现延迟 `appearDelay`

### 参数

#### 出现障碍物

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `appearingObstacle` | GameObject | null | 要出现的障碍物引用 |
| `appearDelay` | float | 0 | 触发后到障碍物出现的延迟（秒），最小值 0 |

#### 触发模式

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `triggerOnce` | bool | true | 是否只触发一次。设为 false 时障碍物在玩家离开后自动隐藏 |
| `repeatResetDelay` | float | 2 | 重复触发模式下的重置延迟（秒），最小值 0 |

#### 检测区域

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `detectionZone` | BoxCollider | null | 检测区域碰撞体 |
| `detectionCenter` | Vector3 | `(0, 0, 0)` | 检测区域中心偏移 |
| `detectionSize` | Vector3 | `(3, 2, 2)` | 检测区域尺寸 |

### 行为流程

1. 玩家进入检测区域 → `triggered = true`，取消待处理的重置
2. 等待 `appearDelay` 秒
3. `appearingObstacle.SetActive(true)` — 障碍物出现
4. `obstacleShown = true`

**重复触发模式（`triggerOnce = false`）：**

5. 玩家离开检测区域 → 启动重置协程
6. 等待 `obstacleShown == true` 且玩家不在区域内后
7. 再等待 `repeatResetDelay` 秒
8. 若玩家仍未进入 → 障碍物重新隐藏（`SetActive(false)`）
9. 状态全部重置，陷阱可再次触发

### 玩家识别逻辑
- 碰撞体 Tag 为 `"Player"`
- 或碰撞体及其父对象上存在 `CharacterController` 组件

---

## 通用注意事项

### 命名空间
所有陷阱脚本统一在 `CountdownTraps` 命名空间下。

### 触发方式
- 所有陷阱均通过 `OnTriggerEnter` 检测玩家进入
- 检测区域使用 `BoxCollider`（`isTrigger = true`）

### 玩家识别
各脚本的玩家识别逻辑大体一致，但存在细微差异：
- **MovingPlatformTrap / CageDropTrap / DisappearingBlockTrap / AppearingObstacleTrap:** 检查 Tag 为 `"Player"` 或存在 `CharacterController`
- **MemePusherTrap / DirectionalWallTrap / DelayedImpulseTrap:** 额外检查 `CharacterController` 的 Tag 也需为 `"Player"`
- 建议同时设置 **Player Tag** 和 **CharacterController 组件** 以确保所有陷阱正常工作

### 重置行为
| 陷阱 | 是否可重置 | 备注 |
|------|-----------|------|
| MovingPlatformTrap | 否 | 一次性触发 |
| MemePusherTrap | 否 | 一次性触发 |
| CageDropTrap | 否 | 一次性触发 |
| DirectionalWallTrap | 是 | 玩家离开后自动重置 |
| DelayedImpulseTrap | 可选 | `triggerOnce` 控制，支持重复触发 |
| DisappearingBlockTrap | 可选 | `triggerOnce` 控制，支持重复触发 |
| AppearingObstacleTrap | 可选 | `triggerOnce` 控制，支持重复触发 |

### 编辑器辅助
- 推荐为每个陷阱挂载 `TrapDetectionZoneGizmo` 以在 Scene 视图中可视化检测区域
- 推荐在 Player 上挂载 `DebugReturnToSafety` 以方便调试（按 1 键回安全点）

### 依赖关系
- `DirectionalWallTrap` 依赖 `FS_ThirdPerson.LocomotionInputManager` 组件
- `MemePusherTrap`、`DirectionalWallTrap`、`DelayedImpulseTrap` 使用 `CharacterController.Move()` 推动玩家
- `MovingPlatformTrap` 通过移动平台 Transform 间接移动玩家
- `DelayedImpulseTrap` 使用 `Physics.OverlapBox` 检测玩家是否在效果区域内
- `DisappearingBlockTrap` 和 `AppearingObstacleTrap` 仅通过 `SetActive` 控制物体的显隐，不直接操作玩家
