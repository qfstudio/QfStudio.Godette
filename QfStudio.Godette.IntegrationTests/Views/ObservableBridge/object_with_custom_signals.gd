extends Node


signal my_signal
signal my_signal_1arg(arg1: String)
signal my_signal_2args(arg1: String, arg2: int)
signal my_signal_3args(arg1: String, arg2: int, arg3: float)
signal my_signal_4args(arg1: String, arg2: int, arg3: float, arg4: bool)
signal my_signal_5args(arg1: String, arg2: int, arg3: float, arg4: bool, arg5: Vector2)
signal my_signal_6args(arg1: String, arg2: int, arg3: float, arg4: bool, arg5: Vector2, arg6: Color)
signal my_signal_7args(arg1: String, arg2: int, arg3: float, arg4: bool, arg5: Vector2, arg6: Color, arg7: NodePath)


func emit_my_signal() -> void:
	my_signal.emit()


func emit_my_signal_1arg() -> void:
	my_signal_1arg.emit("hello")


func emit_my_signal_2args() -> void:
	my_signal_2args.emit("hello", 42)


func emit_my_signal_3args() -> void:
	my_signal_3args.emit("hello", 42, 3.14)


func emit_my_signal_4args() -> void:
	my_signal_4args.emit("hello", 42, 3.14, true)


func emit_my_signal_5args() -> void:
	my_signal_5args.emit("hello", 42, 3.14, true, Vector2(1, 2))


func emit_my_signal_6args() -> void:
	my_signal_6args.emit("hello", 42, 3.14, true, Vector2(1, 2), Color.RED)


func emit_my_signal_7args() -> void:
	my_signal_7args.emit("hello", 42, 3.14, true, Vector2(1, 2), Color.RED, NodePath("."))
