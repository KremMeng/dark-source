

## Codely Structured Memories

undefined
- [2026-11-13 18:47:21] 2026-11-13 头发各向异性准备：复制 Kazuko_hair 为 GO hair_straightUV（原件 SetActive(false) 隐藏备份），网格 Kazuko_hair_straightUV.asset，UV 重展为**按骨骼链分组、每组一条竖向 UV 条带（u=条带槽位按顶点数加权、v=0..1 完整发根→发梢连续深度=父骨深度+主骨插值）**、条带内 U=绕头部方位角排序；占位贴图 Kazuko_hair_straightUV.png（灰白 v 渐变发根暗→发梢亮+竖纹），材质 kazuko_hair_straightUV.mat（复制 kazuko_hair 换贴图，URP/ToonShading）；副本骨骼引用与原件相同——动画/用户的 MagicaCloth BoneCloth 自动同步到副本；**后续做各向异性头发高光请基于这条链分组 UV（v=头发长度轴）**

### User

### Feedback

### Project
- [2026-11-12 20:32:57] 玩家角色链路：SampleScene → Assets/Arts/Prefabs/PlayerHandle.prefab → Assets/Prefabs/Kazuko.prefab（Suriyun Kazuko FBX、Humanoid 导入、根缩放 1.10、绑定姿态斜向站立且前倾）。**Why:** 项目里存在多套 PlayerHandle/Kazuko prefab，易改错对象。**How to apply:** 改角色模型/外观只改 Assets/Prefabs/Kazuko.prefab。2026-11-12 新增裙下防走光白色灯笼短裤 GO "Bloomers"：数据驱动生成（实测 Kazuko_skin+BikiniB 各高度横截面：髋部椭圆+两腿圆的豆形放壳，射线法求并集边界；前/侧/后半径分别按 y 淡出——臀部后侧保持到 y0.74 否则布料陷进臀体积不可见），腿筒带灯笼鼓包（y0.70 峰值 +10mm），两条腿底用向内圆顶布料封底（大腿穿孔穿出、任何角度不可透视），蒙皮 Root_M(上部)+Hip_L/R(腿部按 lat 混合)，网格 Kazuko_Bloomers.asset、材质 kazuko_bloomers.mat。2026-11-12 定稿的泡泡安全裤参数：裤长到 y0.72（盖过臀底），鼓量极小（腿筒 puff 仅 0.009+0.011*gather@hemY+0.045，比平角内裤略鼓），腰头 y≥1.00 处半径×0.97 橡筋收束，贴图为白+裤脚细褶+腰头缩褶；裙摆分区实测（后侧裙摆高达 y0.94、前 0.845、侧片 0.51）——后侧露出的白裤正是防走光用途；**流苏后侧剪影测量必须包含 Kazuko_Bloomers，否则流苏顶部(腰后 back≈0.11)会与安全裤相交出黑斑**。2026-11-12 最终定稿的裙后双流苏 SkirtTassel_L/R：**侧视弧线=红线参考的四分之一圆外凸下垂**（贝塞尔 y/back：起点(1.075,~0.10)→P1(0.95,0.10)先垂直下落→P2(0.70,0.30)渐进外凸→末端(0.48,0.35)小腿中段、离身体一个臀宽多、指向后下方30-45°、无上翘无S形；曾经 back 0.5+ 的大后掠"裙撑"方案被此参考取代）；末端**完全水平平切**（末行5列 y 钳制到同一世界高度）、单层面片（无燕尾无尖角，**燕尾谷点 p -= TangentAt*notch 会导致90°翻折成两层——已整体移除**）；13行×5列×双面=130顶点；骨骼链 **4 节**（SkirtTasselBone_L1-4/R1-4，t=1,1/3,2/3等分至 tMin=0，权重 boneFloat=(1-t)*3）；纯暖金贴图（(1.0,0.87,0.56)+拉丝条纹+渐变），材质：古铜描边(0.38,0.26,0.11)/宽0.12、暖金高光(1.0,0.92,0.72)/0.35/0.0045；挂点测量=armor+skin+bloomers（**头发不计入**——绑定姿态后飘0.3会拽飞挂点）；顶部挂裙甲后侧顶边上方、背面八字分开(顶±0.065→尾±0.15)、面片沿弧线扭转(顶朝后尾朝侧)；SampleScene 根下用户的 MagicaCloth "Tassel"(BoneCloth) rootBones=两链根——**骨骼每次删除重建都会让场景侧引用悬空（报 20409 CreateCloth_CanNotStart），必须重新指向 SkirtTasselBone_L1/R1 并保存场景**
- [2026-11-14 01:32:23] [2026-11-14] 2026-11-14 桥接实操补充：进/出 Play 模式必须 ExecuteMenuItem("Edit/Play")（桥接上下文里 isPlaying=true/delayCall 均被静默忽略）；桥接 REPL 编译失败整段拒跑、成员名不能猜（AnimatorStateInfo 无 shortName，用 shortNameHash/IsName）；AnimatorControllerLayer.iKPass 必须 copy-out 后 ctrl.layers=layers 写回才生效；Play 模式可运行时 new AnimatorController 换 runtimeAnimatorController 隔离测试动画


### Reference
- [2026-11-11 17:54:06] Unity Bridge TCP 直连协议（Unity Tools 工具未注册时可用 Node 直连编辑器，端口见 .com-unity-codely.json 的 unity_port）：连接后收 "WELCOME UNITY-TCP 1 FRAMING=1 SERVER_VERSION=2"（\n 结尾），需回帧 "CLIENT_VERSION=2" 和 "PLATFORM=cli"；帧=8 字节大端长度前缀+UTF8 JSON，命令 {"type":"execute_csharp_script","params":{"script","summary","execution_mode":"editor|play","capture_logs"},"request_id":"ntb-1"}，响应帧含 request_id 按其匹配；另支持 read_console(action:get)、manage_screenshot(action:capture_asset, assets:[路径], view:cardinal, path, filename) 等 type；编辑器占用时无法用 batch 模式，此协议是唯一的编辑器内执行通道
