extends Line2D


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	get_window().mode = Window.MODE_EXCLUSIVE_FULLSCREEN;


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	if (Input.is_key_pressed(KEY_F11)):
		if get_window().mode == Window.MODE_EXCLUSIVE_FULLSCREEN: 
			get_window().mode = Window.MODE_WINDOWED 
		else:
			get_window().mode = Window.MODE_EXCLUSIVE_FULLSCREEN;
