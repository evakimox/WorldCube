# WorldCube
利用Unity免费Asset制作的类似于《Moncage》中世界盒子的效果

![示例图](./Cinemachine多相机联动演示.gif)

## 制作思路
* 每个“世界”的模型，拥有自己的Layer和只摄制自身的相机(Base相机)
* 每个“世界相机”的摄制结果，放到各自的RenderTexture上
* 由于我的Cube有5个面，做5个Material，分别赋予RenderTexture
* 设置这5个材质的shader为Sprite，使RenderTexture的呈现不受光照影响
* 制作5个Plane，怼到主相机前面，并随相机一起动
	* 五个Plane的Transform完全一致(叠在一起)
	* Cube上贴Stencil Mask（需要配合RenderTexturePlane的材质一同使用）
	* 每个RenderTexture Plane和贴在Cube上的其中一个stencil一一对应

## 控制
* 用LeanTouch免费版中LeanGesture的GetScreenDelta判断拖动方向
* 每个相机都受GetScreenDelta影响，用这个控制CinemachineFreeLook(以防相机翻过去)
* 用LeanTouch的Tap事件添加Listener的方法判断点击
	* 做两次RayCast
	* 第一次用主相机和ScreenPositon向RenderTexture Plane cast，找到hit.textureCoord
	* 第二次用世界相机和textureCoord和相机宽高的乘积去cast,判断有没有撞击到Collider

