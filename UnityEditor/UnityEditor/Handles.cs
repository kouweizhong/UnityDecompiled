using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class Handles
	{
		internal enum FilterMode
		{
			Off,
			ShowFiltered,
			ShowRest
		}

		public struct DrawingScope : IDisposable
		{
			private bool m_Disposed;

			private Color m_OriginalColor;

			private Matrix4x4 m_OriginalMatrix;

			public Color originalColor
			{
				get
				{
					return this.m_OriginalColor;
				}
			}

			public Matrix4x4 originalMatrix
			{
				get
				{
					return this.m_OriginalMatrix;
				}
			}

			public DrawingScope(Color color)
			{
				this = new Handles.DrawingScope(color, Handles.matrix);
			}

			public DrawingScope(Matrix4x4 matrix)
			{
				this = new Handles.DrawingScope(Handles.color, matrix);
			}

			public DrawingScope(Color color, Matrix4x4 matrix)
			{
				this.m_Disposed = false;
				this.m_OriginalColor = Handles.color;
				this.m_OriginalMatrix = Handles.matrix;
				Handles.matrix = matrix;
				Handles.color = color;
			}

			public void Dispose()
			{
				if (!this.m_Disposed)
				{
					this.m_Disposed = true;
					Handles.color = this.m_OriginalColor;
					Handles.matrix = this.m_OriginalMatrix;
				}
			}
		}

		public delegate void CapFunction(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType);

		[Obsolete("This delegate is obsolete. Use CapFunction instead.")]
		public delegate void DrawCapFunction(int controlID, Vector3 position, Quaternion rotation, float size);

		public delegate float SizeFunction(Vector3 position);

		private enum PlaneHandle
		{
			xzPlane,
			xyPlane,
			yzPlane
		}

		internal static PrefColor s_XAxisColor = new PrefColor("Scene/X Axis", 0.858823538f, 0.243137255f, 0.113725491f, 0.93f);

		internal static PrefColor s_YAxisColor = new PrefColor("Scene/Y Axis", 0.6039216f, 0.9529412f, 0.282352954f, 0.93f);

		internal static PrefColor s_ZAxisColor = new PrefColor("Scene/Z Axis", 0.227450982f, 0.478431374f, 0.972549f, 0.93f);

		internal static PrefColor s_CenterColor = new PrefColor("Scene/Center Axis", 0.8f, 0.8f, 0.8f, 0.93f);

		internal static PrefColor s_SelectedColor = new PrefColor("Scene/Selected Axis", 0.9647059f, 0.9490196f, 0.196078435f, 0.89f);

		internal static PrefColor s_SecondaryColor = new PrefColor("Scene/Guide Line", 0.5f, 0.5f, 0.5f, 0.2f);

		internal static Color staticColor = new Color(0.5f, 0.5f, 0.5f, 0f);

		internal static float staticBlend = 0.6f;

		internal static float backfaceAlphaMultiplier = 0.2f;

		internal static Color s_ColliderHandleColor = new Color(145f, 244f, 139f, 210f) / 255f;

		internal static Color s_ColliderHandleColorDisabled = new Color(84f, 200f, 77f, 140f) / 255f;

		internal static Color s_BoundingBoxHandleColor = new Color(255f, 255f, 255f, 150f) / 255f;

		private const int kMaxDottedLineVertices = 1000;

		internal static int s_SliderHash = "SliderHash".GetHashCode();

		internal static int s_Slider2DHash = "Slider2DHash".GetHashCode();

		internal static int s_FreeRotateHandleHash = "FreeRotateHandleHash".GetHashCode();

		internal static int s_RadiusHandleHash = "RadiusHandleHash".GetHashCode();

		internal static int s_xAxisMoveHandleHash = "xAxisFreeMoveHandleHash".GetHashCode();

		internal static int s_yAxisMoveHandleHash = "yAxisFreeMoveHandleHash".GetHashCode();

		internal static int s_zAxisMoveHandleHash = "xAxisFreeMoveHandleHash".GetHashCode();

		internal static int s_FreeMoveHandleHash = "FreeMoveHandleHash".GetHashCode();

		internal static int s_xzAxisMoveHandleHash = "xzAxisFreeMoveHandleHash".GetHashCode();

		internal static int s_xyAxisMoveHandleHash = "xyAxisFreeMoveHandleHash".GetHashCode();

		internal static int s_yzAxisMoveHandleHash = "yzAxisFreeMoveHandleHash".GetHashCode();

		internal static int s_ScaleSliderHash = "ScaleSliderHash".GetHashCode();

		internal static int s_ScaleValueHandleHash = "ScaleValueHandleHash".GetHashCode();

		internal static int s_DiscHash = "DiscHash".GetHashCode();

		internal static int s_ButtonHash = "ButtonHash".GetHashCode();

		private static Vector3[] s_RectangleCapPointsCache = new Vector3[5];

		internal static Mesh s_CubeMesh;

		internal static Mesh s_SphereMesh;

		internal static Mesh s_ConeMesh;

		internal static Mesh s_CylinderMesh;

		internal static Mesh s_QuadMesh;

		private static Color lineTransparency = new Color(1f, 1f, 1f, 0.75f);

		private static Vector3[] s_RectangleHandlePointsCache = new Vector3[5];

		private const float k_BoneThickness = 0.08f;

		private static Vector3[] verts = new Vector3[]
		{
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero
		};

		private static bool s_FreeMoveMode = false;

		private static Vector3 s_PlanarHandlesOctant = Vector3.one;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache0;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache1;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache2;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache3;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache4;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache5;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache6;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache7;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache8;

		public static Color xAxisColor
		{
			get
			{
				return Handles.s_XAxisColor;
			}
		}

		public static Color yAxisColor
		{
			get
			{
				return Handles.s_YAxisColor;
			}
		}

		public static Color zAxisColor
		{
			get
			{
				return Handles.s_ZAxisColor;
			}
		}

		public static Color centerColor
		{
			get
			{
				return Handles.s_CenterColor;
			}
		}

		public static Color selectedColor
		{
			get
			{
				return Handles.s_SelectedColor;
			}
		}

		public static Color secondaryColor
		{
			get
			{
				return Handles.s_SecondaryColor;
			}
		}

		public static extern bool lighting
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static Color color
		{
			get
			{
				Color result;
				Handles.INTERNAL_get_color(out result);
				return result;
			}
			set
			{
				Handles.INTERNAL_set_color(ref value);
			}
		}

		public static extern CompareFunction zTest
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static Matrix4x4 matrix
		{
			get
			{
				Matrix4x4 result;
				Handles.INTERNAL_get_matrix(out result);
				return result;
			}
			set
			{
				Handles.INTERNAL_set_matrix(ref value);
			}
		}

		public static Matrix4x4 inverseMatrix
		{
			get
			{
				Matrix4x4 result;
				Handles.INTERNAL_get_inverseMatrix(out result);
				return result;
			}
		}

		public Camera currentCamera
		{
			get
			{
				return Camera.current;
			}
			set
			{
				Handles.Internal_SetCurrentCamera(value);
			}
		}

		internal static Color realHandleColor
		{
			get
			{
				return Handles.color * new Color(1f, 1f, 1f, 0.5f) + ((!Handles.lighting) ? new Color(0f, 0f, 0f, 0f) : new Color(0f, 0f, 0f, 0.5f));
			}
		}

		private static Mesh cubeMesh
		{
			get
			{
				if (Handles.s_CubeMesh == null)
				{
					Handles.Init();
				}
				return Handles.s_CubeMesh;
			}
		}

		private static Mesh coneMesh
		{
			get
			{
				if (Handles.s_ConeMesh == null)
				{
					Handles.Init();
				}
				return Handles.s_ConeMesh;
			}
		}

		private static Mesh cylinderMesh
		{
			get
			{
				if (Handles.s_CylinderMesh == null)
				{
					Handles.Init();
				}
				return Handles.s_CylinderMesh;
			}
		}

		private static Mesh quadMesh
		{
			get
			{
				if (Handles.s_QuadMesh == null)
				{
					Handles.Init();
				}
				return Handles.s_QuadMesh;
			}
		}

		private static Mesh sphereMesh
		{
			get
			{
				if (Handles.s_SphereMesh == null)
				{
					Handles.Init();
				}
				return Handles.s_SphereMesh;
			}
		}

		private static bool currentlyDragging
		{
			get
			{
				return GUIUtility.hotControl != 0;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_color(out Color value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_color(ref Color value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_matrix(out Matrix4x4 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_matrix(ref Matrix4x4 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_inverseMatrix(out Matrix4x4 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ClearHandles();

		public static Vector3 PositionHandle(Vector3 position, Quaternion rotation)
		{
			return Handles.DoPositionHandle(position, rotation);
		}

		public static Quaternion RotationHandle(Quaternion rotation, Vector3 position)
		{
			return Handles.DoRotationHandle(rotation, position);
		}

		public static Vector3 ScaleHandle(Vector3 scale, Vector3 position, Quaternion rotation, float size)
		{
			return Handles.DoScaleHandle(scale, position, rotation, size);
		}

		public static float RadiusHandle(Quaternion rotation, Vector3 position, float radius, bool handlesOnly)
		{
			return Handles.DoRadiusHandle(rotation, position, radius, handlesOnly);
		}

		public static float RadiusHandle(Quaternion rotation, Vector3 position, float radius)
		{
			return Handles.DoRadiusHandle(rotation, position, radius, false);
		}

		internal static Vector2 ConeHandle(Quaternion rotation, Vector3 position, Vector2 angleAndRange, float angleScale, float rangeScale, bool handlesOnly)
		{
			return Handles.DoConeHandle(rotation, position, angleAndRange, angleScale, rangeScale, handlesOnly);
		}

		internal static Vector3 ConeFrustrumHandle(Quaternion rotation, Vector3 position, Vector3 radiusAngleRange)
		{
			return Handles.DoConeFrustrumHandle(rotation, position, radiusAngleRange);
		}

		[ExcludeFromDocs]
		public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.CapFunction capFunction, Vector2 snap)
		{
			bool drawHelper = false;
			return Handles.Slider2D(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, capFunction, snap, drawHelper);
		}

		public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.CapFunction capFunction, Vector2 snap, [DefaultValue("false")] bool drawHelper)
		{
			return UnityEditorInternal.Slider2D.Do(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, capFunction, snap, drawHelper);
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap."), ExcludeFromDocs]
		public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, Vector2 snap)
		{
			bool drawHelper = false;
			return Handles.Slider2D(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, Vector2 snap, [DefaultValue("false")] bool drawHelper)
		{
			return UnityEditorInternal.Slider2D.Do(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
		}

		[ExcludeFromDocs]
		public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.CapFunction capFunction, Vector2 snap)
		{
			bool drawHelper = false;
			return Handles.Slider2D(handlePos, handleDir, slideDir1, slideDir2, handleSize, capFunction, snap, drawHelper);
		}

		public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.CapFunction capFunction, Vector2 snap, [DefaultValue("false")] bool drawHelper)
		{
			int controlID = GUIUtility.GetControlID(Handles.s_Slider2DHash, FocusType.Keyboard);
			return UnityEditorInternal.Slider2D.Do(controlID, handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, capFunction, snap, drawHelper);
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap."), ExcludeFromDocs]
		public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, Vector2 snap)
		{
			bool drawHelper = false;
			return Handles.Slider2D(handlePos, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, Vector2 snap, [DefaultValue("false")] bool drawHelper)
		{
			int controlID = GUIUtility.GetControlID(Handles.s_Slider2DHash, FocusType.Keyboard);
			return UnityEditorInternal.Slider2D.Do(controlID, handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
		}

		[ExcludeFromDocs]
		public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.CapFunction capFunction, Vector2 snap)
		{
			bool drawHelper = false;
			return Handles.Slider2D(id, handlePos, handleDir, slideDir1, slideDir2, handleSize, capFunction, snap, drawHelper);
		}

		public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.CapFunction capFunction, Vector2 snap, [DefaultValue("false")] bool drawHelper)
		{
			return UnityEditorInternal.Slider2D.Do(id, handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, capFunction, snap, drawHelper);
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap."), ExcludeFromDocs]
		public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, Vector2 snap)
		{
			bool drawHelper = false;
			return Handles.Slider2D(id, handlePos, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, Vector2 snap, [DefaultValue("false")] bool drawHelper)
		{
			return UnityEditorInternal.Slider2D.Do(id, handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
		}

		[ExcludeFromDocs]
		public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.CapFunction capFunction, float snap)
		{
			bool drawHelper = false;
			return Handles.Slider2D(handlePos, handleDir, slideDir1, slideDir2, handleSize, capFunction, snap, drawHelper);
		}

		public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.CapFunction capFunction, float snap, [DefaultValue("false")] bool drawHelper)
		{
			int controlID = GUIUtility.GetControlID(Handles.s_Slider2DHash, FocusType.Keyboard);
			return Handles.Slider2D(controlID, handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, capFunction, new Vector2(snap, snap), drawHelper);
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap."), ExcludeFromDocs]
		public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, float snap)
		{
			bool drawHelper = false;
			return Handles.Slider2D(handlePos, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, float snap, [DefaultValue("false")] bool drawHelper)
		{
			int controlID = GUIUtility.GetControlID(Handles.s_Slider2DHash, FocusType.Keyboard);
			return Handles.Slider2D(controlID, handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, drawFunc, new Vector2(snap, snap), drawHelper);
		}

		public static Quaternion FreeRotateHandle(Quaternion rotation, Vector3 position, float size)
		{
			int controlID = GUIUtility.GetControlID(Handles.s_FreeRotateHandleHash, FocusType.Keyboard);
			return FreeRotate.Do(controlID, rotation, position, size);
		}

		public static float ScaleSlider(float scale, Vector3 position, Vector3 direction, Quaternion rotation, float size, float snap)
		{
			int controlID = GUIUtility.GetControlID(Handles.s_ScaleSliderHash, FocusType.Keyboard);
			return SliderScale.DoAxis(controlID, scale, position, direction, rotation, size, snap);
		}

		public static Quaternion Disc(Quaternion rotation, Vector3 position, Vector3 axis, float size, bool cutoffPlane, float snap)
		{
			int controlID = GUIUtility.GetControlID(Handles.s_DiscHash, FocusType.Keyboard);
			return UnityEditorInternal.Disc.Do(controlID, rotation, position, axis, size, cutoffPlane, snap);
		}

		internal static void SetupIgnoreRaySnapObjects()
		{
			HandleUtility.ignoreRaySnapObjects = Selection.GetTransforms((SelectionMode)10);
		}

		public static float SnapValue(float val, float snap)
		{
			float result;
			if (EditorGUI.actionKey && snap > 0f)
			{
				result = Mathf.Round(val / snap) * snap;
			}
			else
			{
				result = val;
			}
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawCameraWithGrid(Camera cam, int renderMode, ref DrawGridParameters gridParam);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawCamera(Camera cam, int renderMode);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_FinishDrawingCamera(Camera cam);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ClearCamera(Camera cam);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_SetCurrentCamera(Camera cam);

		internal static void SetSceneViewColors(Color wire, Color wireOverlay, Color selectedOutline, Color selectedWire)
		{
			Handles.INTERNAL_CALL_SetSceneViewColors(ref wire, ref wireOverlay, ref selectedOutline, ref selectedWire);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetSceneViewColors(ref Color wire, ref Color wireOverlay, ref Color selectedOutline, ref Color selectedWire);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void EnableCameraFx(Camera cam, bool fx);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void EnableCameraFlares(Camera cam, bool flares);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void EnableCameraSkybox(Camera cam, bool skybox);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetCameraOnlyDrawMesh(Camera cam);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetupCamera(Camera cam);

		internal static void DrawTwoShadedWireDisc(Vector3 position, Vector3 axis, float radius)
		{
			Color color = Handles.color;
			Color color2 = color;
			color.a *= Handles.backfaceAlphaMultiplier;
			Handles.color = color;
			Handles.DrawWireDisc(position, axis, radius);
			Handles.color = color2;
		}

		internal static void DrawTwoShadedWireDisc(Vector3 position, Vector3 axis, Vector3 from, float degrees, float radius)
		{
			Handles.DrawWireArc(position, axis, from, degrees, radius);
			Color color = Handles.color;
			Color color2 = color;
			color.a *= Handles.backfaceAlphaMultiplier;
			Handles.color = color;
			Handles.DrawWireArc(position, axis, from, degrees - 360f, radius);
			Handles.color = color2;
		}

		internal static Matrix4x4 StartCapDraw(Vector3 position, Quaternion rotation, float size)
		{
			Shader.SetGlobalColor("_HandleColor", Handles.realHandleColor);
			Shader.SetGlobalFloat("_HandleSize", size);
			Matrix4x4 matrix4x = Handles.matrix * Matrix4x4.TRS(position, rotation, Vector3.one);
			Shader.SetGlobalMatrix("_ObjectToWorld", matrix4x);
			HandleUtility.handleMaterial.SetInt("_HandleZTest", (int)Handles.zTest);
			HandleUtility.handleMaterial.SetPass(0);
			return matrix4x;
		}

		[Obsolete("Use CubeHandleCap instead")]
		public static void CubeCap(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Graphics.DrawMeshNow(Handles.cubeMesh, Handles.StartCapDraw(position, rotation, size));
			}
		}

		[Obsolete("Use SphereHandleCap instead")]
		public static void SphereCap(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Graphics.DrawMeshNow(Handles.sphereMesh, Handles.StartCapDraw(position, rotation, size));
			}
		}

		[Obsolete("Use ConeHandleCap instead")]
		public static void ConeCap(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Graphics.DrawMeshNow(Handles.coneMesh, Handles.StartCapDraw(position, rotation, size));
			}
		}

		[Obsolete("Use CylinderHandleCap instead")]
		public static void CylinderCap(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Graphics.DrawMeshNow(Handles.cylinderMesh, Handles.StartCapDraw(position, rotation, size));
			}
		}

		[Obsolete("Use RectangleHandleCap instead")]
		public static void RectangleCap(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			Handles.RectangleCap(controlID, position, rotation, new Vector2(size, size));
		}

		internal static void RectangleCap(int controlID, Vector3 position, Quaternion rotation, Vector2 size)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Vector3 b = rotation * new Vector3(size.x, 0f, 0f);
				Vector3 b2 = rotation * new Vector3(0f, size.y, 0f);
				Handles.s_RectangleCapPointsCache[0] = position + b + b2;
				Handles.s_RectangleCapPointsCache[1] = position + b - b2;
				Handles.s_RectangleCapPointsCache[2] = position - b - b2;
				Handles.s_RectangleCapPointsCache[3] = position - b + b2;
				Handles.s_RectangleCapPointsCache[4] = position + b + b2;
				Handles.DrawPolyLine(Handles.s_RectangleCapPointsCache);
			}
		}

		public static void SelectionFrame(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Handles.StartCapDraw(position, rotation, size);
				Vector3 b = rotation * new Vector3(size, 0f, 0f);
				Vector3 b2 = rotation * new Vector3(0f, size, 0f);
				Vector3 vector = position - b + b2;
				Vector3 vector2 = position + b + b2;
				Vector3 vector3 = position + b - b2;
				Vector3 vector4 = position - b - b2;
				Handles.DrawLine(vector, vector2);
				Handles.DrawLine(vector2, vector3);
				Handles.DrawLine(vector3, vector4);
				Handles.DrawLine(vector4, vector);
			}
		}

		[Obsolete("Use DotHandleCap instead")]
		public static void DotCap(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			if (Event.current.type == EventType.Repaint)
			{
				position = Handles.matrix.MultiplyPoint(position);
				Vector3 b = Camera.current.transform.right * size;
				Vector3 b2 = Camera.current.transform.up * size;
				Color c = Handles.color * new Color(1f, 1f, 1f, 0.99f);
				HandleUtility.ApplyWireMaterial(Handles.zTest);
				GL.Begin(7);
				GL.Color(c);
				GL.Vertex(position + b + b2);
				GL.Vertex(position + b - b2);
				GL.Vertex(position - b - b2);
				GL.Vertex(position - b + b2);
				GL.End();
			}
		}

		[Obsolete("Use CircleHandleCap instead")]
		public static void CircleCap(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Handles.StartCapDraw(position, rotation, size);
				Vector3 normal = rotation * new Vector3(0f, 0f, 1f);
				Handles.DrawWireDisc(position, normal, size);
			}
		}

		[Obsolete("Use ArrowHandleCap instead")]
		public static void ArrowCap(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Vector3 vector = rotation * Vector3.forward;
				Handles.ConeCap(controlID, position + vector * size, Quaternion.LookRotation(vector), size * 0.2f);
				Handles.DrawLine(position, position + vector * size * 0.9f);
			}
		}

		[Obsolete("DrawCylinder has been renamed to CylinderCap.")]
		public static void DrawCylinder(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			Handles.CylinderCap(controlID, position, rotation, size);
		}

		[Obsolete("DrawSphere has been renamed to SphereCap.")]
		public static void DrawSphere(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			Handles.SphereCap(controlID, position, rotation, size);
		}

		[Obsolete("DrawRectangle has been renamed to RectangleCap.")]
		public static void DrawRectangle(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			Handles.RectangleCap(controlID, position, rotation, size);
		}

		[Obsolete("DrawCube has been renamed to CubeCap.")]
		public static void DrawCube(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			Handles.CubeCap(controlID, position, rotation, size);
		}

		[Obsolete("DrawArrow has been renamed to ArrowCap.")]
		public static void DrawArrow(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			Handles.ArrowCap(controlID, position, rotation, size);
		}

		[Obsolete("DrawCone has been renamed to ConeCap.")]
		public static void DrawCone(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			Handles.ConeCap(controlID, position, rotation, size);
		}

		internal static void DrawAAPolyLine(Color[] colors, Vector3[] points)
		{
			Handles.DoDrawAAPolyLine(colors, points, -1, null, 2f, 0.75f);
		}

		internal static void DrawAAPolyLine(float width, Color[] colors, Vector3[] points)
		{
			Handles.DoDrawAAPolyLine(colors, points, -1, null, width, 0.75f);
		}

		public static void DrawAAPolyLine(params Vector3[] points)
		{
			Handles.DoDrawAAPolyLine(null, points, -1, null, 2f, 0.75f);
		}

		public static void DrawAAPolyLine(float width, params Vector3[] points)
		{
			Handles.DoDrawAAPolyLine(null, points, -1, null, width, 0.75f);
		}

		public static void DrawAAPolyLine(Texture2D lineTex, params Vector3[] points)
		{
			Handles.DoDrawAAPolyLine(null, points, -1, lineTex, (float)(lineTex.height / 2), 0.99f);
		}

		public static void DrawAAPolyLine(float width, int actualNumberOfPoints, params Vector3[] points)
		{
			Handles.DoDrawAAPolyLine(null, points, actualNumberOfPoints, null, width, 0.75f);
		}

		public static void DrawAAPolyLine(Texture2D lineTex, float width, params Vector3[] points)
		{
			Handles.DoDrawAAPolyLine(null, points, -1, lineTex, width, 0.99f);
		}

		private static void DoDrawAAPolyLine(Color[] colors, Vector3[] points, int actualNumberOfPoints, Texture2D lineTex, float width, float alpha)
		{
			if (Event.current.type == EventType.Repaint)
			{
				HandleUtility.ApplyWireMaterial(Handles.zTest);
				Color color = new Color(1f, 1f, 1f, alpha);
				if (colors != null)
				{
					for (int i = 0; i < colors.Length; i++)
					{
						colors[i] *= color;
					}
				}
				else
				{
					color *= Handles.color;
				}
				Handles.Internal_DrawAAPolyLine(colors, points, color, actualNumberOfPoints, lineTex, width, Handles.matrix);
			}
		}

		private static void Internal_DrawAAPolyLine(Color[] colors, Vector3[] points, Color defaultColor, int actualNumberOfPoints, Texture2D texture, float width, Matrix4x4 toWorld)
		{
			Handles.INTERNAL_CALL_Internal_DrawAAPolyLine(colors, points, ref defaultColor, actualNumberOfPoints, texture, width, ref toWorld);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_DrawAAPolyLine(Color[] colors, Vector3[] points, ref Color defaultColor, int actualNumberOfPoints, Texture2D texture, float width, ref Matrix4x4 toWorld);

		public static void DrawAAConvexPolygon(params Vector3[] points)
		{
			Handles.DoDrawAAConvexPolygon(points, -1, 1f);
		}

		private static void DoDrawAAConvexPolygon(Vector3[] points, int actualNumberOfPoints, float alpha)
		{
			if (Event.current.type == EventType.Repaint)
			{
				HandleUtility.ApplyWireMaterial(Handles.zTest);
				Color defaultColor = new Color(1f, 1f, 1f, alpha) * Handles.color;
				Handles.Internal_DrawAAConvexPolygon(points, defaultColor, actualNumberOfPoints, Handles.matrix);
			}
		}

		private static void Internal_DrawAAConvexPolygon(Vector3[] points, Color defaultColor, int actualNumberOfPoints, Matrix4x4 toWorld)
		{
			Handles.INTERNAL_CALL_Internal_DrawAAConvexPolygon(points, ref defaultColor, actualNumberOfPoints, ref toWorld);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_DrawAAConvexPolygon(Vector3[] points, ref Color defaultColor, int actualNumberOfPoints, ref Matrix4x4 toWorld);

		public static void DrawBezier(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, Color color, Texture2D texture, float width)
		{
			if (Event.current.type == EventType.Repaint)
			{
				HandleUtility.ApplyWireMaterial(Handles.zTest);
				Handles.Internal_DrawBezier(startPosition, endPosition, startTangent, endTangent, color, texture, width, Handles.matrix);
			}
		}

		private static void Internal_DrawBezier(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, Color color, Texture2D texture, float width, Matrix4x4 toWorld)
		{
			Handles.INTERNAL_CALL_Internal_DrawBezier(ref startPosition, ref endPosition, ref startTangent, ref endTangent, ref color, texture, width, ref toWorld);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_DrawBezier(ref Vector3 startPosition, ref Vector3 endPosition, ref Vector3 startTangent, ref Vector3 endTangent, ref Color color, Texture2D texture, float width, ref Matrix4x4 toWorld);

		public static void DrawWireDisc(Vector3 center, Vector3 normal, float radius)
		{
			Vector3 from = Vector3.Cross(normal, Vector3.up);
			if (from.sqrMagnitude < 0.001f)
			{
				from = Vector3.Cross(normal, Vector3.right);
			}
			Handles.DrawWireArc(center, normal, from, 360f, radius);
		}

		public static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
		{
			Vector3[] array = new Vector3[60];
			Handles.SetDiscSectionPoints(array, center, normal, from, angle, radius);
			Handles.DrawPolyLine(array);
		}

		public static void DrawSolidRectangleWithOutline(Rect rectangle, Color faceColor, Color outlineColor)
		{
			Vector3[] array = new Vector3[]
			{
				new Vector3(rectangle.xMin, rectangle.yMin, 0f),
				new Vector3(rectangle.xMax, rectangle.yMin, 0f),
				new Vector3(rectangle.xMax, rectangle.yMax, 0f),
				new Vector3(rectangle.xMin, rectangle.yMax, 0f)
			};
			Handles.DrawSolidRectangleWithOutline(array, faceColor, outlineColor);
		}

		public static void DrawSolidRectangleWithOutline(Vector3[] verts, Color faceColor, Color outlineColor)
		{
			if (Event.current.type == EventType.Repaint)
			{
				HandleUtility.ApplyWireMaterial(Handles.zTest);
				GL.PushMatrix();
				GL.MultMatrix(Handles.matrix);
				if (faceColor.a > 0f)
				{
					Color c = faceColor * Handles.color;
					GL.Begin(4);
					for (int i = 0; i < 2; i++)
					{
						GL.Color(c);
						GL.Vertex(verts[i * 2]);
						GL.Vertex(verts[i * 2 + 1]);
						GL.Vertex(verts[(i * 2 + 2) % 4]);
						GL.Vertex(verts[i * 2]);
						GL.Vertex(verts[(i * 2 + 2) % 4]);
						GL.Vertex(verts[i * 2 + 1]);
					}
					GL.End();
				}
				if (outlineColor.a > 0f)
				{
					Color c2 = outlineColor * Handles.color;
					GL.Begin(1);
					GL.Color(c2);
					for (int j = 0; j < 4; j++)
					{
						GL.Vertex(verts[j]);
						GL.Vertex(verts[(j + 1) % 4]);
					}
					GL.End();
				}
				GL.PopMatrix();
			}
		}

		public static void DrawSolidDisc(Vector3 center, Vector3 normal, float radius)
		{
			Vector3 from = Vector3.Cross(normal, Vector3.up);
			if (from.sqrMagnitude < 0.001f)
			{
				from = Vector3.Cross(normal, Vector3.right);
			}
			Handles.DrawSolidArc(center, normal, from, 360f, radius);
		}

		public static void DrawSolidArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Vector3[] array = new Vector3[60];
				Handles.SetDiscSectionPoints(array, center, normal, from, angle, radius);
				Shader.SetGlobalColor("_HandleColor", Handles.color * new Color(1f, 1f, 1f, 0.5f));
				Shader.SetGlobalFloat("_HandleSize", 1f);
				HandleUtility.ApplyWireMaterial(Handles.zTest);
				GL.PushMatrix();
				GL.MultMatrix(Handles.matrix);
				GL.Begin(4);
				for (int i = 1; i < array.Length; i++)
				{
					GL.Color(Handles.color);
					GL.Vertex(center);
					GL.Vertex(array[i - 1]);
					GL.Vertex(array[i]);
					GL.Vertex(center);
					GL.Vertex(array[i]);
					GL.Vertex(array[i - 1]);
				}
				GL.End();
				GL.PopMatrix();
			}
		}

		internal static void SetDiscSectionPoints(Vector3[] dest, Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
		{
			Handles.INTERNAL_CALL_SetDiscSectionPoints(dest, ref center, ref normal, ref from, angle, radius);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetDiscSectionPoints(Vector3[] dest, ref Vector3 center, ref Vector3 normal, ref Vector3 from, float angle, float radius);

		internal static void Init()
		{
			if (!Handles.s_CubeMesh)
			{
				GameObject gameObject = (GameObject)EditorGUIUtility.Load("SceneView/HandlesGO.fbx");
				if (!gameObject)
				{
					Debug.Log("Couldn't find SceneView/HandlesGO.fbx");
				}
				gameObject.SetActive(false);
				IEnumerator enumerator = gameObject.transform.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Transform transform = (Transform)enumerator.Current;
						MeshFilter component = transform.GetComponent<MeshFilter>();
						string name = transform.name;
						if (name != null)
						{
							if (!(name == "Cube"))
							{
								if (!(name == "Sphere"))
								{
									if (!(name == "Cone"))
									{
										if (!(name == "Cylinder"))
										{
											if (name == "Quad")
											{
												Handles.s_QuadMesh = component.sharedMesh;
											}
										}
										else
										{
											Handles.s_CylinderMesh = component.sharedMesh;
										}
									}
									else
									{
										Handles.s_ConeMesh = component.sharedMesh;
									}
								}
								else
								{
									Handles.s_SphereMesh = component.sharedMesh;
								}
							}
							else
							{
								Handles.s_CubeMesh = component.sharedMesh;
							}
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				if (Application.platform == RuntimePlatform.WindowsEditor)
				{
					Handles.ReplaceFontForWindows((Font)EditorGUIUtility.LoadRequired(EditorResourcesUtility.fontsPath + "Lucida Grande.ttf"));
					Handles.ReplaceFontForWindows((Font)EditorGUIUtility.LoadRequired(EditorResourcesUtility.fontsPath + "Lucida Grande Bold.ttf"));
					Handles.ReplaceFontForWindows((Font)EditorGUIUtility.LoadRequired(EditorResourcesUtility.fontsPath + "Lucida Grande Small.ttf"));
					Handles.ReplaceFontForWindows((Font)EditorGUIUtility.LoadRequired(EditorResourcesUtility.fontsPath + "Lucida Grande Small Bold.ttf"));
					Handles.ReplaceFontForWindows((Font)EditorGUIUtility.LoadRequired(EditorResourcesUtility.fontsPath + "Lucida Grande Big.ttf"));
				}
			}
		}

		private static void ReplaceFontForWindows(Font font)
		{
			if (font.name.Contains("Bold"))
			{
				font.fontNames = new string[]
				{
					"Verdana Bold",
					"Tahoma Bold"
				};
			}
			else
			{
				font.fontNames = new string[]
				{
					"Verdana",
					"Tahoma"
				};
			}
			font.hideFlags = HideFlags.HideAndDontSave;
		}

		public static void Label(Vector3 position, string text)
		{
			Handles.Label(position, EditorGUIUtility.TempContent(text), GUI.skin.label);
		}

		public static void Label(Vector3 position, Texture image)
		{
			Handles.Label(position, EditorGUIUtility.TempContent(image), GUI.skin.label);
		}

		public static void Label(Vector3 position, GUIContent content)
		{
			Handles.Label(position, content, GUI.skin.label);
		}

		public static void Label(Vector3 position, string text, GUIStyle style)
		{
			Handles.Label(position, EditorGUIUtility.TempContent(text), style);
		}

		public static void Label(Vector3 position, GUIContent content, GUIStyle style)
		{
			Handles.BeginGUI();
			GUI.Label(HandleUtility.WorldPointToSizedRect(position, content, style), content, style);
			Handles.EndGUI();
		}

		internal static Rect GetCameraRect(Rect position)
		{
			Rect rect = GUIClip.Unclip(position);
			Rect result = new Rect(rect.xMin, (float)Screen.height - rect.yMax, rect.width, rect.height);
			return result;
		}

		public static Vector2 GetMainGameViewSize()
		{
			return GameView.GetMainGameViewTargetSize();
		}

		public static void ClearCamera(Rect position, Camera camera)
		{
			Event current = Event.current;
			if (camera.targetTexture == null)
			{
				Rect rect = GUIClip.Unclip(position);
				rect = EditorGUIUtility.PointsToPixels(rect);
				Rect pixelRect = new Rect(rect.xMin, (float)Screen.height - rect.yMax, rect.width, rect.height);
				camera.pixelRect = pixelRect;
			}
			else
			{
				camera.rect = new Rect(0f, 0f, 1f, 1f);
			}
			if (current.type == EventType.Repaint)
			{
				Handles.Internal_ClearCamera(camera);
			}
			else
			{
				Handles.Internal_SetCurrentCamera(camera);
			}
		}

		internal static void DrawCameraImpl(Rect position, Camera camera, DrawCameraMode drawMode, bool drawGrid, DrawGridParameters gridParam, bool finish)
		{
			Event current = Event.current;
			if (current.type == EventType.Repaint)
			{
				if (camera.targetTexture == null)
				{
					Rect rect = GUIClip.Unclip(position);
					rect = EditorGUIUtility.PointsToPixels(rect);
					camera.pixelRect = new Rect(rect.xMin, (float)Screen.height - rect.yMax, rect.width, rect.height);
				}
				else
				{
					camera.rect = new Rect(0f, 0f, 1f, 1f);
				}
				if (drawMode == DrawCameraMode.Normal)
				{
					RenderTexture targetTexture = camera.targetTexture;
					camera.targetTexture = RenderTexture.active;
					camera.Render();
					camera.targetTexture = targetTexture;
				}
				else
				{
					if (drawGrid)
					{
						Handles.Internal_DrawCameraWithGrid(camera, (int)drawMode, ref gridParam);
					}
					else
					{
						Handles.Internal_DrawCamera(camera, (int)drawMode);
					}
					if (finish)
					{
						Handles.Internal_FinishDrawingCamera(camera);
					}
				}
			}
			else
			{
				Handles.Internal_SetCurrentCamera(camera);
			}
		}

		internal static void DrawCamera(Rect position, Camera camera, DrawCameraMode drawMode, DrawGridParameters gridParam)
		{
			Handles.DrawCameraImpl(position, camera, drawMode, true, gridParam, true);
		}

		internal static void DrawCameraStep1(Rect position, Camera camera, DrawCameraMode drawMode, DrawGridParameters gridParam)
		{
			Handles.DrawCameraImpl(position, camera, drawMode, true, gridParam, false);
		}

		internal static void DrawCameraStep2(Camera camera, DrawCameraMode drawMode)
		{
			if (Event.current.type == EventType.Repaint && drawMode != DrawCameraMode.Normal)
			{
				Handles.Internal_FinishDrawingCamera(camera);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void EmitGUIGeometryForCamera(Camera source, Camera dest);

		[ExcludeFromDocs]
		public static void DrawCamera(Rect position, Camera camera)
		{
			DrawCameraMode drawMode = DrawCameraMode.Normal;
			Handles.DrawCamera(position, camera, drawMode);
		}

		public static void DrawCamera(Rect position, Camera camera, [DefaultValue("DrawCameraMode.Normal")] DrawCameraMode drawMode)
		{
			Handles.DrawCameraImpl(position, camera, drawMode, false, default(DrawGridParameters), true);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetCameraFilterMode(Camera camera, Handles.FilterMode mode);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Handles.FilterMode GetCameraFilterMode(Camera camera);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DrawCameraFade(Camera camera, float fade);

		public static void SetCamera(Camera camera)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Handles.Internal_SetupCamera(camera);
			}
			else
			{
				Handles.Internal_SetCurrentCamera(camera);
			}
		}

		public static void SetCamera(Rect position, Camera camera)
		{
			Rect rect = GUIClip.Unclip(position);
			rect = EditorGUIUtility.PointsToPixels(rect);
			Rect pixelRect = new Rect(rect.xMin, (float)Screen.height - rect.yMax, rect.width, rect.height);
			camera.pixelRect = pixelRect;
			Event current = Event.current;
			if (current.type == EventType.Repaint)
			{
				Handles.Internal_SetupCamera(camera);
			}
			else
			{
				Handles.Internal_SetCurrentCamera(camera);
			}
		}

		public static void BeginGUI()
		{
			if (Camera.current && Event.current.type == EventType.Repaint)
			{
				GUIClip.Reapply();
			}
		}

		[Obsolete("Please use BeginGUI() with GUILayout.BeginArea(position) / GUILayout.EndArea()")]
		public static void BeginGUI(Rect position)
		{
			GUILayout.BeginArea(position);
		}

		public static void EndGUI()
		{
			Camera current = Camera.current;
			if (current && Event.current.type == EventType.Repaint)
			{
				Handles.Internal_SetupCamera(current);
			}
		}

		internal static void ShowStaticLabelIfNeeded(Vector3 pos)
		{
			if (!Tools.s_Hidden && EditorApplication.isPlaying && GameObjectUtility.ContainsStatic(Selection.gameObjects))
			{
				Handles.ShowStaticLabel(pos);
			}
		}

		internal static void ShowStaticLabel(Vector3 pos)
		{
			Handles.color = Color.white;
			Handles.zTest = CompareFunction.Always;
			GUIStyle gUIStyle = "SC ViewAxisLabel";
			gUIStyle.alignment = TextAnchor.MiddleLeft;
			gUIStyle.fixedWidth = 0f;
			Handles.BeginGUI();
			Rect position = HandleUtility.WorldPointToSizedRect(pos, EditorGUIUtility.TempContent("Static"), gUIStyle);
			position.x += 10f;
			position.y += 10f;
			GUI.Label(position, EditorGUIUtility.TempContent("Static"), gUIStyle);
			Handles.EndGUI();
		}

		private static Vector3[] Internal_MakeBezierPoints(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, int division)
		{
			return Handles.INTERNAL_CALL_Internal_MakeBezierPoints(ref startPosition, ref endPosition, ref startTangent, ref endTangent, division);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Vector3[] INTERNAL_CALL_Internal_MakeBezierPoints(ref Vector3 startPosition, ref Vector3 endPosition, ref Vector3 startTangent, ref Vector3 endTangent, int division);

		public static Vector3[] MakeBezierPoints(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, int division)
		{
			if (division < 1)
			{
				throw new ArgumentOutOfRangeException("division", "Must be greater than zero");
			}
			return Handles.Internal_MakeBezierPoints(startPosition, endPosition, startTangent, endTangent, division);
		}

		private static bool BeginLineDrawing(Matrix4x4 matrix, bool dottedLines, int mode)
		{
			bool result;
			if (Event.current.type != EventType.Repaint)
			{
				result = false;
			}
			else
			{
				Color c = Handles.color * Handles.lineTransparency;
				if (dottedLines)
				{
					HandleUtility.ApplyDottedWireMaterial(Handles.zTest);
				}
				else
				{
					HandleUtility.ApplyWireMaterial(Handles.zTest);
				}
				GL.PushMatrix();
				GL.MultMatrix(matrix);
				GL.Begin(mode);
				GL.Color(c);
				result = true;
			}
			return result;
		}

		private static void EndLineDrawing()
		{
			GL.End();
			GL.PopMatrix();
		}

		public static void DrawPolyLine(params Vector3[] points)
		{
			if (Handles.BeginLineDrawing(Handles.matrix, false, 2))
			{
				for (int i = 0; i < points.Length; i++)
				{
					GL.Vertex(points[i]);
				}
				Handles.EndLineDrawing();
			}
		}

		public static void DrawLine(Vector3 p1, Vector3 p2)
		{
			if (Handles.BeginLineDrawing(Handles.matrix, false, 1))
			{
				GL.Vertex(p1);
				GL.Vertex(p2);
				Handles.EndLineDrawing();
			}
		}

		public static void DrawLines(Vector3[] lineSegments)
		{
			if (Handles.BeginLineDrawing(Handles.matrix, false, 1))
			{
				for (int i = 0; i < lineSegments.Length; i += 2)
				{
					Vector3 v = lineSegments[i];
					Vector3 v2 = lineSegments[i + 1];
					GL.Vertex(v);
					GL.Vertex(v2);
				}
				Handles.EndLineDrawing();
			}
		}

		public static void DrawLines(Vector3[] points, int[] segmentIndices)
		{
			if (Handles.BeginLineDrawing(Handles.matrix, false, 1))
			{
				for (int i = 0; i < segmentIndices.Length; i += 2)
				{
					Vector3 v = points[segmentIndices[i]];
					Vector3 v2 = points[segmentIndices[i + 1]];
					GL.Vertex(v);
					GL.Vertex(v2);
				}
				Handles.EndLineDrawing();
			}
		}

		public static void DrawDottedLine(Vector3 p1, Vector3 p2, float screenSpaceSize)
		{
			if (Handles.BeginLineDrawing(Handles.matrix, true, 1))
			{
				float x = screenSpaceSize * EditorGUIUtility.pixelsPerPoint;
				GL.MultiTexCoord(1, p1);
				GL.MultiTexCoord2(2, x, 0f);
				GL.Vertex(p1);
				GL.MultiTexCoord(1, p1);
				GL.MultiTexCoord2(2, x, 0f);
				GL.Vertex(p2);
				Handles.EndLineDrawing();
			}
		}

		public static void DrawDottedLines(Vector3[] lineSegments, float screenSpaceSize)
		{
			if (Handles.BeginLineDrawing(Handles.matrix, true, 1))
			{
				float x = screenSpaceSize * EditorGUIUtility.pixelsPerPoint;
				for (int i = 0; i < lineSegments.Length; i += 2)
				{
					Vector3 v = lineSegments[i];
					Vector3 v2 = lineSegments[i + 1];
					GL.MultiTexCoord(1, v);
					GL.MultiTexCoord2(2, x, 0f);
					GL.Vertex(v);
					GL.MultiTexCoord(1, v);
					GL.MultiTexCoord2(2, x, 0f);
					GL.Vertex(v2);
				}
				Handles.EndLineDrawing();
			}
		}

		public static void DrawDottedLines(Vector3[] points, int[] segmentIndices, float screenSpaceSize)
		{
			if (Handles.BeginLineDrawing(Handles.matrix, true, 1))
			{
				float x = screenSpaceSize * EditorGUIUtility.pixelsPerPoint;
				for (int i = 0; i < segmentIndices.Length; i += 2)
				{
					Vector3 v = points[segmentIndices[i]];
					Vector3 v2 = points[segmentIndices[i + 1]];
					GL.MultiTexCoord(1, v);
					GL.MultiTexCoord2(2, x, 0f);
					GL.Vertex(v);
					GL.MultiTexCoord(1, v);
					GL.MultiTexCoord2(2, x, 0f);
					GL.Vertex(v2);
				}
				Handles.EndLineDrawing();
			}
		}

		public static void DrawWireCube(Vector3 center, Vector3 size)
		{
			Vector3 vector = size * 0.5f;
			Vector3[] array = new Vector3[]
			{
				center + new Vector3(-vector.x, -vector.y, -vector.z),
				center + new Vector3(-vector.x, vector.y, -vector.z),
				center + new Vector3(vector.x, vector.y, -vector.z),
				center + new Vector3(vector.x, -vector.y, -vector.z),
				center + new Vector3(-vector.x, -vector.y, -vector.z),
				center + new Vector3(-vector.x, -vector.y, vector.z),
				center + new Vector3(-vector.x, vector.y, vector.z),
				center + new Vector3(vector.x, vector.y, vector.z),
				center + new Vector3(vector.x, -vector.y, vector.z),
				center + new Vector3(-vector.x, -vector.y, vector.z)
			};
			Handles.DrawPolyLine(array);
			Handles.DrawLine(array[1], array[6]);
			Handles.DrawLine(array[2], array[7]);
			Handles.DrawLine(array[3], array[8]);
		}

		public static Vector3 Slider(Vector3 position, Vector3 direction)
		{
			float arg_2B_2 = HandleUtility.GetHandleSize(position);
			if (Handles.<>f__mg$cache0 == null)
			{
				Handles.<>f__mg$cache0 = new Handles.CapFunction(Handles.ArrowHandleCap);
			}
			return Handles.Slider(position, direction, arg_2B_2, Handles.<>f__mg$cache0, -1f);
		}

		public static Vector3 Slider(Vector3 position, Vector3 direction, float size, Handles.CapFunction capFunction, float snap)
		{
			int controlID = GUIUtility.GetControlID(Handles.s_SliderHash, FocusType.Keyboard);
			return Slider1D.Do(controlID, position, direction, size, capFunction, snap);
		}

		public static Vector3 Slider(int controlID, Vector3 position, Vector3 direction, float size, Handles.CapFunction capFunction, float snap)
		{
			return Slider1D.Do(controlID, position, direction, size, capFunction, snap);
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		public static Vector3 Slider(Vector3 position, Vector3 direction, float size, Handles.DrawCapFunction drawFunc, float snap)
		{
			int controlID = GUIUtility.GetControlID(Handles.s_SliderHash, FocusType.Keyboard);
			return Slider1D.Do(controlID, position, direction, size, drawFunc, snap);
		}

		public static Vector3 FreeMoveHandle(Vector3 position, Quaternion rotation, float size, Vector3 snap, Handles.CapFunction capFunction)
		{
			int controlID = GUIUtility.GetControlID(Handles.s_FreeMoveHandleHash, FocusType.Keyboard);
			return FreeMove.Do(controlID, position, rotation, size, snap, capFunction);
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		public static Vector3 FreeMoveHandle(Vector3 position, Quaternion rotation, float size, Vector3 snap, Handles.DrawCapFunction capFunc)
		{
			int controlID = GUIUtility.GetControlID(Handles.s_FreeMoveHandleHash, FocusType.Keyboard);
			return FreeMove.Do(controlID, position, rotation, size, snap, capFunc);
		}

		public static float ScaleValueHandle(float value, Vector3 position, Quaternion rotation, float size, Handles.CapFunction capFunction, float snap)
		{
			int controlID = GUIUtility.GetControlID(Handles.s_ScaleValueHandleHash, FocusType.Keyboard);
			return SliderScale.DoCenter(controlID, value, position, rotation, size, capFunction, snap);
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		public static float ScaleValueHandle(float value, Vector3 position, Quaternion rotation, float size, Handles.DrawCapFunction capFunc, float snap)
		{
			int controlID = GUIUtility.GetControlID(Handles.s_ScaleValueHandleHash, FocusType.Keyboard);
			return SliderScale.DoCenter(controlID, value, position, rotation, size, capFunc, snap);
		}

		public static bool Button(Vector3 position, Quaternion direction, float size, float pickSize, Handles.CapFunction capFunction)
		{
			int controlID = GUIUtility.GetControlID(Handles.s_ButtonHash, FocusType.Passive);
			return UnityEditorInternal.Button.Do(controlID, position, direction, size, pickSize, capFunction);
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		public static bool Button(Vector3 position, Quaternion direction, float size, float pickSize, Handles.DrawCapFunction capFunc)
		{
			int controlID = GUIUtility.GetControlID(Handles.s_ButtonHash, FocusType.Passive);
			return UnityEditorInternal.Button.Do(controlID, position, direction, size, pickSize, capFunc);
		}

		internal static bool Button(int controlID, Vector3 position, Quaternion direction, float size, float pickSize, Handles.CapFunction capFunction)
		{
			return UnityEditorInternal.Button.Do(controlID, position, direction, size, pickSize, capFunction);
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		internal static bool Button(int controlID, Vector3 position, Quaternion direction, float size, float pickSize, Handles.DrawCapFunction capFunc)
		{
			return UnityEditorInternal.Button.Do(controlID, position, direction, size, pickSize, capFunc);
		}

		public static void CubeHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (eventType != EventType.Layout)
			{
				if (eventType == EventType.Repaint)
				{
					Graphics.DrawMeshNow(Handles.cubeMesh, Handles.StartCapDraw(position, rotation, size));
				}
			}
			else
			{
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size));
			}
		}

		public static void SphereHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (eventType != EventType.Layout)
			{
				if (eventType == EventType.Repaint)
				{
					Graphics.DrawMeshNow(Handles.sphereMesh, Handles.StartCapDraw(position, rotation, size));
				}
			}
			else
			{
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size));
			}
		}

		public static void ConeHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (eventType != EventType.Layout)
			{
				if (eventType == EventType.Repaint)
				{
					Graphics.DrawMeshNow(Handles.coneMesh, Handles.StartCapDraw(position, rotation, size));
				}
			}
			else
			{
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size));
			}
		}

		public static void CylinderHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (eventType != EventType.Layout)
			{
				if (eventType == EventType.Repaint)
				{
					Graphics.DrawMeshNow(Handles.cylinderMesh, Handles.StartCapDraw(position, rotation, size));
				}
			}
			else
			{
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size));
			}
		}

		public static void RectangleHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			Handles.RectangleHandleCap(controlID, position, rotation, new Vector2(size, size), eventType);
		}

		internal static void RectangleHandleCap(int controlID, Vector3 position, Quaternion rotation, Vector2 size, EventType eventType)
		{
			if (eventType != EventType.Layout)
			{
				if (eventType == EventType.Repaint)
				{
					Vector3 b = rotation * new Vector3(size.x, 0f, 0f);
					Vector3 b2 = rotation * new Vector3(0f, size.y, 0f);
					Handles.s_RectangleHandlePointsCache[0] = position + b + b2;
					Handles.s_RectangleHandlePointsCache[1] = position + b - b2;
					Handles.s_RectangleHandlePointsCache[2] = position - b - b2;
					Handles.s_RectangleHandlePointsCache[3] = position - b + b2;
					Handles.s_RectangleHandlePointsCache[4] = position + b + b2;
					Handles.DrawPolyLine(Handles.s_RectangleHandlePointsCache);
				}
			}
			else
			{
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToRectangleInternal(position, rotation, size));
			}
		}

		public static void DotHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (eventType != EventType.Layout)
			{
				if (eventType == EventType.Repaint)
				{
					position = Handles.matrix.MultiplyPoint(position);
					Vector3 b = Camera.current.transform.right * size;
					Vector3 b2 = Camera.current.transform.up * size;
					Color c = Handles.color * new Color(1f, 1f, 1f, 0.99f);
					HandleUtility.ApplyWireMaterial();
					GL.Begin(7);
					GL.Color(c);
					GL.Vertex(position + b + b2);
					GL.Vertex(position + b - b2);
					GL.Vertex(position - b - b2);
					GL.Vertex(position - b + b2);
					GL.End();
				}
			}
			else
			{
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToRectangle(position, rotation, size));
			}
		}

		public static void CircleHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (eventType != EventType.Layout)
			{
				if (eventType == EventType.Repaint)
				{
					Handles.StartCapDraw(position, rotation, size);
					Vector3 normal = rotation * new Vector3(0f, 0f, 1f);
					Handles.DrawWireDisc(position, normal, size);
				}
			}
			else
			{
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToRectangle(position, rotation, size));
			}
		}

		public static void ArrowHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (eventType != EventType.Layout)
			{
				if (eventType == EventType.Repaint)
				{
					Vector3 vector = rotation * Vector3.forward;
					Handles.ConeHandleCap(controlID, position + vector * size, Quaternion.LookRotation(vector), size * 0.2f, eventType);
					Handles.DrawLine(position, position + vector * size * 0.9f);
				}
			}
			else
			{
				Vector3 a = rotation * Vector3.forward;
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToLine(position, position + a * size * 0.9f));
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position + a * size, size * 0.2f));
			}
		}

		public static void DrawSelectionFrame(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (eventType == EventType.Repaint)
			{
				Handles.StartCapDraw(position, rotation, size);
				Vector3 b = rotation * new Vector3(size, 0f, 0f);
				Vector3 b2 = rotation * new Vector3(0f, size, 0f);
				Vector3 vector = position - b + b2;
				Vector3 vector2 = position + b + b2;
				Vector3 vector3 = position + b - b2;
				Vector3 vector4 = position - b - b2;
				Handles.DrawLine(vector, vector2);
				Handles.DrawLine(vector2, vector3);
				Handles.DrawLine(vector3, vector4);
				Handles.DrawLine(vector4, vector);
			}
		}

		internal static float DistanceToPolygone(Vector3[] vertices)
		{
			return HandleUtility.DistanceToPolyLine(vertices);
		}

		internal static void DoBoneHandle(Transform target)
		{
			Handles.DoBoneHandle(target, null);
		}

		internal static void DoBoneHandle(Transform target, Dictionary<Transform, bool> validBones)
		{
			int hashCode = target.name.GetHashCode();
			Event current = Event.current;
			bool flag = false;
			if (validBones != null)
			{
				IEnumerator enumerator = target.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Transform key = (Transform)enumerator.Current;
						if (validBones.ContainsKey(key))
						{
							flag = true;
							break;
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
			Vector3 position = target.position;
			List<Vector3> list = new List<Vector3>();
			if (!flag && target.parent != null)
			{
				list.Add(target.position + (target.position - target.parent.position) * 0.4f);
			}
			else
			{
				IEnumerator enumerator2 = target.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						Transform transform = (Transform)enumerator2.Current;
						if (validBones == null || validBones.ContainsKey(transform))
						{
							list.Add(transform.position);
						}
					}
				}
				finally
				{
					IDisposable disposable2;
					if ((disposable2 = (enumerator2 as IDisposable)) != null)
					{
						disposable2.Dispose();
					}
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				Vector3 vector = list[i];
				switch (current.GetTypeForControl(hashCode))
				{
				case EventType.MouseDown:
					if (!current.alt && ((HandleUtility.nearestControl == hashCode && current.button == 0) || (GUIUtility.keyboardControl == hashCode && current.button == 2)))
					{
						int num = hashCode;
						GUIUtility.keyboardControl = num;
						GUIUtility.hotControl = num;
						if (current.shift)
						{
							UnityEngine.Object[] objects = Selection.objects;
							if (!ArrayUtility.Contains<UnityEngine.Object>(objects, target))
							{
								ArrayUtility.Add<UnityEngine.Object>(ref objects, target);
								Selection.objects = objects;
							}
						}
						else
						{
							Selection.activeObject = target;
						}
						EditorGUIUtility.PingObject(target);
						current.Use();
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == hashCode && (current.button == 0 || current.button == 2))
					{
						GUIUtility.hotControl = 0;
						current.Use();
					}
					break;
				case EventType.MouseDrag:
					if (!current.alt && GUIUtility.hotControl == hashCode)
					{
						DragAndDrop.PrepareStartDrag();
						DragAndDrop.objectReferences = new UnityEngine.Object[]
						{
							target
						};
						DragAndDrop.StartDrag(ObjectNames.GetDragAndDropTitle(target));
						current.Use();
					}
					break;
				case EventType.Repaint:
				{
					float num2 = Vector3.Magnitude(vector - position);
					if (num2 > 0f)
					{
						float num3 = num2 * 0.08f;
						if (flag)
						{
							Handles.DrawBone(vector, position, num3);
						}
						else
						{
							Handles.SphereHandleCap(hashCode, position, target.rotation, num3 * 0.2f, EventType.Repaint);
						}
					}
					break;
				}
				case EventType.Layout:
				{
					float num4 = Vector3.Magnitude(vector - position);
					float radius = num4 * 0.08f;
					Vector3[] boneVertices = Handles.GetBoneVertices(vector, position, radius);
					HandleUtility.AddControl(hashCode, Handles.DistanceToPolygone(boneVertices));
					break;
				}
				}
			}
		}

		internal static void DrawBone(Vector3 endPoint, Vector3 basePoint, float size)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Vector3[] boneVertices = Handles.GetBoneVertices(endPoint, basePoint, size);
				HandleUtility.ApplyWireMaterial();
				GL.Begin(4);
				GL.Color(Handles.color);
				for (int i = 0; i < 3; i++)
				{
					GL.Vertex(boneVertices[i * 6]);
					GL.Vertex(boneVertices[i * 6 + 1]);
					GL.Vertex(boneVertices[i * 6 + 2]);
					GL.Vertex(boneVertices[i * 6 + 3]);
					GL.Vertex(boneVertices[i * 6 + 4]);
					GL.Vertex(boneVertices[i * 6 + 5]);
				}
				GL.End();
				GL.Begin(1);
				GL.Color(Handles.color * new Color(1f, 1f, 1f, 0f) + new Color(0f, 0f, 0f, 1f));
				for (int j = 0; j < 3; j++)
				{
					GL.Vertex(boneVertices[j * 6]);
					GL.Vertex(boneVertices[j * 6 + 1]);
					GL.Vertex(boneVertices[j * 6 + 1]);
					GL.Vertex(boneVertices[j * 6 + 2]);
				}
				GL.End();
			}
		}

		internal static Vector3[] GetBoneVertices(Vector3 endPoint, Vector3 basePoint, float radius)
		{
			Vector3 lhs = Vector3.Normalize(endPoint - basePoint);
			Vector3 vector = Vector3.Cross(lhs, Vector3.up);
			if (Vector3.SqrMagnitude(vector) < 0.1f)
			{
				vector = Vector3.Cross(lhs, Vector3.right);
			}
			vector.Normalize();
			Vector3 a = Vector3.Cross(lhs, vector);
			Vector3[] array = new Vector3[18];
			float num = 0f;
			for (int i = 0; i < 3; i++)
			{
				float num2 = Mathf.Cos(num);
				float num3 = Mathf.Sin(num);
				float num4 = Mathf.Cos(num + 2.09439516f);
				float num5 = Mathf.Sin(num + 2.09439516f);
				Vector3 vector2 = basePoint + vector * (num2 * radius) + a * (num3 * radius);
				Vector3 vector3 = basePoint + vector * (num4 * radius) + a * (num5 * radius);
				array[i * 6] = endPoint;
				array[i * 6 + 1] = vector2;
				array[i * 6 + 2] = vector3;
				array[i * 6 + 3] = basePoint;
				array[i * 6 + 4] = vector3;
				array[i * 6 + 5] = vector2;
				num += 2.09439516f;
			}
			return array;
		}

		internal static Vector3 DoConeFrustrumHandle(Quaternion rotation, Vector3 position, Vector3 radiusAngleRange)
		{
			Vector3 vector = rotation * Vector3.forward;
			Vector3 vector2 = rotation * Vector3.up;
			Vector3 vector3 = rotation * Vector3.right;
			float num = radiusAngleRange.x;
			float num2 = radiusAngleRange.y;
			float num3 = radiusAngleRange.z;
			num2 = Mathf.Max(0f, num2);
			bool changed = GUI.changed;
			num3 = Handles.SizeSlider(position, vector, num3);
			GUI.changed |= changed;
			changed = GUI.changed;
			GUI.changed = false;
			num = Handles.SizeSlider(position, vector2, num);
			num = Handles.SizeSlider(position, -vector2, num);
			num = Handles.SizeSlider(position, vector3, num);
			num = Handles.SizeSlider(position, -vector3, num);
			if (GUI.changed)
			{
				num = Mathf.Max(0f, num);
			}
			GUI.changed |= changed;
			changed = GUI.changed;
			GUI.changed = false;
			float num4 = Mathf.Min(1000f, Mathf.Abs(num3 * Mathf.Tan(0.0174532924f * num2)) + num);
			num4 = Handles.SizeSlider(position + vector * num3, vector2, num4);
			num4 = Handles.SizeSlider(position + vector * num3, -vector2, num4);
			num4 = Handles.SizeSlider(position + vector * num3, vector3, num4);
			num4 = Handles.SizeSlider(position + vector * num3, -vector3, num4);
			if (GUI.changed)
			{
				num2 = Mathf.Clamp(57.29578f * Mathf.Atan((num4 - num) / Mathf.Abs(num3)), 0f, 90f);
			}
			GUI.changed |= changed;
			if (num > 0f)
			{
				Handles.DrawWireDisc(position, vector, num);
			}
			if (num4 > 0f)
			{
				Handles.DrawWireDisc(position + num3 * vector, vector, num4);
			}
			Handles.DrawLine(position + vector2 * num, position + vector * num3 + vector2 * num4);
			Handles.DrawLine(position - vector2 * num, position + vector * num3 - vector2 * num4);
			Handles.DrawLine(position + vector3 * num, position + vector * num3 + vector3 * num4);
			Handles.DrawLine(position - vector3 * num, position + vector * num3 - vector3 * num4);
			return new Vector3(num, num2, num3);
		}

		internal static Vector2 DoConeHandle(Quaternion rotation, Vector3 position, Vector2 angleAndRange, float angleScale, float rangeScale, bool handlesOnly)
		{
			float num = angleAndRange.x;
			float num2 = angleAndRange.y;
			float num3 = num2 * rangeScale;
			Vector3 vector = rotation * Vector3.forward;
			Vector3 vector2 = rotation * Vector3.up;
			Vector3 vector3 = rotation * Vector3.right;
			bool changed = GUI.changed;
			GUI.changed = false;
			num3 = Handles.SizeSlider(position, vector, num3);
			if (GUI.changed)
			{
				num2 = Mathf.Max(0f, num3 / rangeScale);
			}
			GUI.changed |= changed;
			changed = GUI.changed;
			GUI.changed = false;
			float num4 = num3 * Mathf.Tan(0.0174532924f * num / 2f) * angleScale;
			num4 = Handles.SizeSlider(position + vector * num3, vector2, num4);
			num4 = Handles.SizeSlider(position + vector * num3, -vector2, num4);
			num4 = Handles.SizeSlider(position + vector * num3, vector3, num4);
			num4 = Handles.SizeSlider(position + vector * num3, -vector3, num4);
			if (GUI.changed)
			{
				num = Mathf.Clamp(57.29578f * Mathf.Atan(num4 / (num3 * angleScale)) * 2f, 0f, 179f);
			}
			GUI.changed |= changed;
			if (!handlesOnly)
			{
				Handles.DrawLine(position, position + vector * num3 + vector2 * num4);
				Handles.DrawLine(position, position + vector * num3 - vector2 * num4);
				Handles.DrawLine(position, position + vector * num3 + vector3 * num4);
				Handles.DrawLine(position, position + vector * num3 - vector3 * num4);
				Handles.DrawWireDisc(position + num3 * vector, vector, num4);
			}
			return new Vector2(num, num2);
		}

		private static float SizeSlider(Vector3 p, Vector3 d, float r)
		{
			Vector3 vector = p + d * r;
			float handleSize = HandleUtility.GetHandleSize(vector);
			bool changed = GUI.changed;
			GUI.changed = false;
			Vector3 arg_4D_0 = vector;
			float arg_4D_2 = handleSize * 0.03f;
			if (Handles.<>f__mg$cache1 == null)
			{
				Handles.<>f__mg$cache1 = new Handles.CapFunction(Handles.DotHandleCap);
			}
			vector = Handles.Slider(arg_4D_0, d, arg_4D_2, Handles.<>f__mg$cache1, 0f);
			if (GUI.changed)
			{
				r = Vector3.Dot(vector - p, d);
			}
			GUI.changed |= changed;
			return r;
		}

		public static Vector3 DoPositionHandle(Vector3 position, Quaternion rotation)
		{
			Event current = Event.current;
			EventType type = current.type;
			Vector3 result;
			if (type != EventType.KeyDown)
			{
				if (type == EventType.KeyUp)
				{
					position = Handles.DoPositionHandle_Internal(position, rotation);
					if (current.keyCode == KeyCode.V && !current.shift && !Handles.currentlyDragging)
					{
						Handles.s_FreeMoveMode = false;
					}
					result = position;
					return result;
				}
				if (type == EventType.Layout)
				{
					if (!Handles.currentlyDragging && !Tools.vertexDragging)
					{
						Handles.s_FreeMoveMode = current.shift;
					}
				}
			}
			else if (current.keyCode == KeyCode.V && !Handles.currentlyDragging)
			{
				Handles.s_FreeMoveMode = true;
			}
			result = Handles.DoPositionHandle_Internal(position, rotation);
			return result;
		}

		private static Vector3 DoPositionHandle_Internal(Vector3 position, Quaternion rotation)
		{
			float handleSize = HandleUtility.GetHandleSize(position);
			Color color = Handles.color;
			bool flag = !Tools.s_Hidden && EditorApplication.isPlaying && GameObjectUtility.ContainsStatic(Selection.gameObjects);
			Handles.color = ((!flag) ? Handles.xAxisColor : Color.Lerp(Handles.xAxisColor, Handles.staticColor, Handles.staticBlend));
			GUI.SetNextControlName("xAxis");
			Vector3 arg_9A_0 = position;
			Vector3 arg_9A_1 = rotation * Vector3.right;
			float arg_9A_2 = handleSize;
			if (Handles.<>f__mg$cache2 == null)
			{
				Handles.<>f__mg$cache2 = new Handles.CapFunction(Handles.ArrowHandleCap);
			}
			position = Handles.Slider(arg_9A_0, arg_9A_1, arg_9A_2, Handles.<>f__mg$cache2, SnapSettings.move.x);
			Handles.color = ((!flag) ? Handles.yAxisColor : Color.Lerp(Handles.yAxisColor, Handles.staticColor, Handles.staticBlend));
			GUI.SetNextControlName("yAxis");
			Vector3 arg_10C_0 = position;
			Vector3 arg_10C_1 = rotation * Vector3.up;
			float arg_10C_2 = handleSize;
			if (Handles.<>f__mg$cache3 == null)
			{
				Handles.<>f__mg$cache3 = new Handles.CapFunction(Handles.ArrowHandleCap);
			}
			position = Handles.Slider(arg_10C_0, arg_10C_1, arg_10C_2, Handles.<>f__mg$cache3, SnapSettings.move.y);
			Handles.color = ((!flag) ? Handles.zAxisColor : Color.Lerp(Handles.zAxisColor, Handles.staticColor, Handles.staticBlend));
			GUI.SetNextControlName("zAxis");
			Vector3 arg_17E_0 = position;
			Vector3 arg_17E_1 = rotation * Vector3.forward;
			float arg_17E_2 = handleSize;
			if (Handles.<>f__mg$cache4 == null)
			{
				Handles.<>f__mg$cache4 = new Handles.CapFunction(Handles.ArrowHandleCap);
			}
			position = Handles.Slider(arg_17E_0, arg_17E_1, arg_17E_2, Handles.<>f__mg$cache4, SnapSettings.move.z);
			if (Handles.s_FreeMoveMode)
			{
				Handles.color = Handles.centerColor;
				GUI.SetNextControlName("FreeMoveAxis");
				Vector3 arg_1CF_0 = position;
				float arg_1CF_2 = handleSize * 0.15f;
				Vector3 arg_1CF_3 = SnapSettings.move;
				if (Handles.<>f__mg$cache5 == null)
				{
					Handles.<>f__mg$cache5 = new Handles.CapFunction(Handles.RectangleHandleCap);
				}
				position = Handles.FreeMoveHandle(arg_1CF_0, rotation, arg_1CF_2, arg_1CF_3, Handles.<>f__mg$cache5);
			}
			else
			{
				position = Handles.DoPlanarHandle(Handles.PlaneHandle.xzPlane, position, rotation, handleSize * 0.25f);
				position = Handles.DoPlanarHandle(Handles.PlaneHandle.xyPlane, position, rotation, handleSize * 0.25f);
				position = Handles.DoPlanarHandle(Handles.PlaneHandle.yzPlane, position, rotation, handleSize * 0.25f);
			}
			Handles.color = color;
			return position;
		}

		private static Vector3 DoPlanarHandle(Handles.PlaneHandle planeID, Vector3 position, Quaternion rotation, float handleSize)
		{
			int num = 0;
			int num2 = 0;
			int hint = 0;
			bool flag = !Tools.s_Hidden && EditorApplication.isPlaying && GameObjectUtility.ContainsStatic(Selection.gameObjects);
			if (planeID != Handles.PlaneHandle.xzPlane)
			{
				if (planeID != Handles.PlaneHandle.xyPlane)
				{
					if (planeID == Handles.PlaneHandle.yzPlane)
					{
						num = 1;
						num2 = 2;
						Handles.color = ((!flag) ? Handles.xAxisColor : Handles.staticColor);
						hint = Handles.s_yzAxisMoveHandleHash;
					}
				}
				else
				{
					num = 0;
					num2 = 1;
					Handles.color = ((!flag) ? Handles.zAxisColor : Handles.staticColor);
					hint = Handles.s_xyAxisMoveHandleHash;
				}
			}
			else
			{
				num = 0;
				num2 = 2;
				Handles.color = ((!flag) ? Handles.yAxisColor : Handles.staticColor);
				hint = Handles.s_xzAxisMoveHandleHash;
			}
			int index = 3 - num2 - num;
			Color color = Handles.color;
			Matrix4x4 matrix4x = Matrix4x4.TRS(position, rotation, Vector3.one);
			Vector3 result;
			if (Camera.current == null)
			{
				result = position;
			}
			else
			{
				Vector3 normalized;
				if (Camera.current.orthographic)
				{
					normalized = matrix4x.inverse.MultiplyVector(Camera.current.transform.rotation * -Vector3.forward).normalized;
				}
				else
				{
					normalized = matrix4x.inverse.MultiplyPoint(Camera.current.transform.position).normalized;
				}
				int controlID = GUIUtility.GetControlID(hint, FocusType.Keyboard);
				if (Mathf.Abs(normalized[index]) < 0.05f && GUIUtility.hotControl != controlID)
				{
					Handles.color = color;
					result = position;
				}
				else
				{
					if (!Handles.currentlyDragging)
					{
						Handles.s_PlanarHandlesOctant[num] = (float)((normalized[num] >= -0.01f) ? 1 : -1);
						Handles.s_PlanarHandlesOctant[num2] = (float)((normalized[num2] >= -0.01f) ? 1 : -1);
					}
					Vector3 vector = Handles.s_PlanarHandlesOctant;
					vector[index] = 0f;
					vector = rotation * (vector * handleSize * 0.5f);
					Vector3 vector2 = Vector3.zero;
					Vector3 vector3 = Vector3.zero;
					Vector3 vector4 = Vector3.zero;
					vector2[num] = 1f;
					vector3[num2] = 1f;
					vector4[index] = 1f;
					vector2 = rotation * vector2;
					vector3 = rotation * vector3;
					vector4 = rotation * vector4;
					Handles.verts[0] = position + vector + (vector2 + vector3) * handleSize * 0.5f;
					Handles.verts[1] = position + vector + (-vector2 + vector3) * handleSize * 0.5f;
					Handles.verts[2] = position + vector + (-vector2 - vector3) * handleSize * 0.5f;
					Handles.verts[3] = position + vector + (vector2 - vector3) * handleSize * 0.5f;
					Handles.DrawSolidRectangleWithOutline(Handles.verts, new Color(Handles.color.r, Handles.color.g, Handles.color.b, 0.1f), new Color(0f, 0f, 0f, 0f));
					int arg_413_0 = controlID;
					Vector3 arg_413_1 = position;
					Vector3 arg_413_2 = vector;
					Vector3 arg_413_3 = vector4;
					Vector3 arg_413_4 = vector2;
					Vector3 arg_413_5 = vector3;
					float arg_413_6 = handleSize * 0.5f;
					if (Handles.<>f__mg$cache6 == null)
					{
						Handles.<>f__mg$cache6 = new Handles.CapFunction(Handles.RectangleHandleCap);
					}
					position = Handles.Slider2D(arg_413_0, arg_413_1, arg_413_2, arg_413_3, arg_413_4, arg_413_5, arg_413_6, Handles.<>f__mg$cache6, new Vector2(SnapSettings.move[num], SnapSettings.move[num2]));
					Handles.color = color;
					result = position;
				}
			}
			return result;
		}

		internal static float DoRadiusHandle(Quaternion rotation, Vector3 position, float radius, bool handlesOnly)
		{
			float num = 90f;
			Vector3[] array = new Vector3[]
			{
				rotation * Vector3.right,
				rotation * Vector3.up,
				rotation * Vector3.forward,
				rotation * -Vector3.right,
				rotation * -Vector3.up,
				rotation * -Vector3.forward
			};
			Vector3 vector;
			if (Camera.current.orthographic)
			{
				vector = Camera.current.transform.forward;
				if (!handlesOnly)
				{
					Handles.DrawWireDisc(position, vector, radius);
					for (int i = 0; i < 3; i++)
					{
						Vector3 normalized = Vector3.Cross(array[i], vector).normalized;
						Handles.DrawTwoShadedWireDisc(position, array[i], normalized, 180f, radius);
					}
				}
			}
			else
			{
				vector = position - Matrix4x4.Inverse(Handles.matrix).MultiplyPoint(Camera.current.transform.position);
				float sqrMagnitude = vector.sqrMagnitude;
				float num2 = radius * radius;
				float num3 = num2 * num2 / sqrMagnitude;
				float num4 = num3 / num2;
				if (num4 < 1f)
				{
					float num5 = Mathf.Sqrt(num2 - num3);
					num = Mathf.Atan2(num5, Mathf.Sqrt(num3)) * 57.29578f;
					if (!handlesOnly)
					{
						Handles.DrawWireDisc(position - num2 * vector / sqrMagnitude, vector, num5);
					}
				}
				else
				{
					num = -1000f;
				}
				if (!handlesOnly)
				{
					for (int j = 0; j < 3; j++)
					{
						if (num4 < 1f)
						{
							float num6 = Vector3.Angle(vector, array[j]);
							num6 = 90f - Mathf.Min(num6, 180f - num6);
							float num7 = Mathf.Tan(num6 * 0.0174532924f);
							float num8 = Mathf.Sqrt(num3 + num7 * num7 * num3) / radius;
							if (num8 < 1f)
							{
								float num9 = Mathf.Asin(num8) * 57.29578f;
								Vector3 vector2 = Vector3.Cross(array[j], vector).normalized;
								vector2 = Quaternion.AngleAxis(num9, array[j]) * vector2;
								Handles.DrawTwoShadedWireDisc(position, array[j], vector2, (90f - num9) * 2f, radius);
							}
							else
							{
								Handles.DrawTwoShadedWireDisc(position, array[j], radius);
							}
						}
						else
						{
							Handles.DrawTwoShadedWireDisc(position, array[j], radius);
						}
					}
				}
			}
			Color color = Handles.color;
			for (int k = 0; k < 6; k++)
			{
				int controlID = GUIUtility.GetControlID(Handles.s_RadiusHandleHash, FocusType.Keyboard);
				float num10 = Vector3.Angle(array[k], -vector);
				if ((num10 > 5f && num10 < 175f) || GUIUtility.hotControl == controlID)
				{
					Color color2 = color;
					if (num10 > num + 5f)
					{
						color2.a = Mathf.Clamp01(Handles.backfaceAlphaMultiplier * color.a * 2f);
					}
					else
					{
						color2.a = Mathf.Clamp01(color.a * 2f);
					}
					Handles.color = color2;
					Vector3 vector3 = position + radius * array[k];
					bool changed = GUI.changed;
					GUI.changed = false;
					int arg_41A_0 = controlID;
					Vector3 arg_41A_1 = vector3;
					Vector3 arg_41A_2 = array[k];
					float arg_41A_3 = HandleUtility.GetHandleSize(vector3) * 0.03f;
					if (Handles.<>f__mg$cache7 == null)
					{
						Handles.<>f__mg$cache7 = new Handles.CapFunction(Handles.DotHandleCap);
					}
					vector3 = Slider1D.Do(arg_41A_0, arg_41A_1, arg_41A_2, arg_41A_3, Handles.<>f__mg$cache7, 0f);
					if (GUI.changed)
					{
						radius = Vector3.Distance(vector3, position);
					}
					GUI.changed |= changed;
				}
			}
			Handles.color = color;
			return radius;
		}

		internal static Vector2 DoRectHandles(Quaternion rotation, Vector3 position, Vector2 size)
		{
			Vector3 b = rotation * Vector3.forward;
			Vector3 vector = rotation * Vector3.up;
			Vector3 vector2 = rotation * Vector3.right;
			float num = 0.5f * size.x;
			float num2 = 0.5f * size.y;
			Vector3 vector3 = position + vector * num2 + vector2 * num;
			Vector3 vector4 = position - vector * num2 + vector2 * num;
			Vector3 vector5 = position - vector * num2 - vector2 * num;
			Vector3 vector6 = position + vector * num2 - vector2 * num;
			Handles.DrawLine(vector3, vector4);
			Handles.DrawLine(vector4, vector5);
			Handles.DrawLine(vector5, vector6);
			Handles.DrawLine(vector6, vector3);
			Color color = Handles.color;
			color.a = Mathf.Clamp01(color.a * 2f);
			Handles.color = color;
			num2 = Handles.SizeSlider(position, vector, num2);
			num2 = Handles.SizeSlider(position, -vector, num2);
			num = Handles.SizeSlider(position, vector2, num);
			num = Handles.SizeSlider(position, -vector2, num);
			if ((Tools.current != Tool.Move && Tools.current != Tool.Scale) || Tools.pivotRotation != PivotRotation.Local)
			{
				Handles.DrawLine(position, position + b);
			}
			size.x = 2f * num;
			size.y = 2f * num2;
			return size;
		}

		public static Quaternion DoRotationHandle(Quaternion rotation, Vector3 position)
		{
			float handleSize = HandleUtility.GetHandleSize(position);
			Color color = Handles.color;
			bool flag = !Tools.s_Hidden && EditorApplication.isPlaying && GameObjectUtility.ContainsStatic(Selection.gameObjects);
			Handles.color = ((!flag) ? Handles.xAxisColor : Color.Lerp(Handles.xAxisColor, Handles.staticColor, Handles.staticBlend));
			rotation = Handles.Disc(rotation, position, rotation * Vector3.right, handleSize, true, SnapSettings.rotation);
			Handles.color = ((!flag) ? Handles.yAxisColor : Color.Lerp(Handles.yAxisColor, Handles.staticColor, Handles.staticBlend));
			rotation = Handles.Disc(rotation, position, rotation * Vector3.up, handleSize, true, SnapSettings.rotation);
			Handles.color = ((!flag) ? Handles.zAxisColor : Color.Lerp(Handles.zAxisColor, Handles.staticColor, Handles.staticBlend));
			rotation = Handles.Disc(rotation, position, rotation * Vector3.forward, handleSize, true, SnapSettings.rotation);
			if (!flag)
			{
				Handles.color = Handles.centerColor;
				rotation = Handles.Disc(rotation, position, Camera.current.transform.forward, handleSize * 1.1f, false, 0f);
				rotation = Handles.FreeRotateHandle(rotation, position, handleSize);
			}
			Handles.color = color;
			return rotation;
		}

		public static Vector3 DoScaleHandle(Vector3 scale, Vector3 position, Quaternion rotation, float size)
		{
			bool flag = !Tools.s_Hidden && EditorApplication.isPlaying && GameObjectUtility.ContainsStatic(Selection.gameObjects);
			Handles.color = ((!flag) ? Handles.xAxisColor : Color.Lerp(Handles.xAxisColor, Handles.staticColor, Handles.staticBlend));
			scale.x = Handles.ScaleSlider(scale.x, position, rotation * Vector3.right, rotation, size, SnapSettings.scale);
			Handles.color = ((!flag) ? Handles.yAxisColor : Color.Lerp(Handles.yAxisColor, Handles.staticColor, Handles.staticBlend));
			scale.y = Handles.ScaleSlider(scale.y, position, rotation * Vector3.up, rotation, size, SnapSettings.scale);
			Handles.color = ((!flag) ? Handles.zAxisColor : Color.Lerp(Handles.zAxisColor, Handles.staticColor, Handles.staticBlend));
			scale.z = Handles.ScaleSlider(scale.z, position, rotation * Vector3.forward, rotation, size, SnapSettings.scale);
			Handles.color = Handles.centerColor;
			EditorGUI.BeginChangeCheck();
			float arg_14B_0 = scale.x;
			if (Handles.<>f__mg$cache8 == null)
			{
				Handles.<>f__mg$cache8 = new Handles.CapFunction(Handles.CubeHandleCap);
			}
			float num = Handles.ScaleValueHandle(arg_14B_0, position, rotation, size, Handles.<>f__mg$cache8, SnapSettings.scale);
			if (EditorGUI.EndChangeCheck())
			{
				float num2 = num / scale.x;
				scale.x = num;
				scale.y *= num2;
				scale.z *= num2;
			}
			return scale;
		}

		internal static float DoSimpleEdgeHandle(Quaternion rotation, Vector3 position, float radius)
		{
			Vector3 vector = rotation * Vector3.right;
			EditorGUI.BeginChangeCheck();
			radius = Handles.SizeSlider(position, vector, radius);
			radius = Handles.SizeSlider(position, -vector, radius);
			if (EditorGUI.EndChangeCheck())
			{
				radius = Mathf.Max(0f, radius);
			}
			if (radius > 0f)
			{
				Handles.DrawLine(position - vector * radius, position + vector * radius);
			}
			return radius;
		}

		internal static float DoSimpleRadiusHandle(Quaternion rotation, Vector3 position, float radius, bool hemisphere)
		{
			Vector3 vector = rotation * Vector3.forward;
			Vector3 vector2 = rotation * Vector3.up;
			Vector3 vector3 = rotation * Vector3.right;
			bool changed = GUI.changed;
			GUI.changed = false;
			radius = Handles.SizeSlider(position, vector, radius);
			if (!hemisphere)
			{
				radius = Handles.SizeSlider(position, -vector, radius);
			}
			if (GUI.changed)
			{
				radius = Mathf.Max(0f, radius);
			}
			GUI.changed |= changed;
			changed = GUI.changed;
			GUI.changed = false;
			radius = Handles.SizeSlider(position, vector2, radius);
			radius = Handles.SizeSlider(position, -vector2, radius);
			radius = Handles.SizeSlider(position, vector3, radius);
			radius = Handles.SizeSlider(position, -vector3, radius);
			if (GUI.changed)
			{
				radius = Mathf.Max(0f, radius);
			}
			GUI.changed |= changed;
			if (radius > 0f)
			{
				Handles.DrawWireDisc(position, vector, radius);
				Handles.DrawWireArc(position, vector2, -vector3, (float)((!hemisphere) ? 360 : 180), radius);
				Handles.DrawWireArc(position, vector3, vector2, (float)((!hemisphere) ? 360 : 180), radius);
			}
			return radius;
		}
	}
}
