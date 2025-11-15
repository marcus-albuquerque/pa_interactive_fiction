extends Control

@export var command_processor: Node
@export var input_node: LineEdit
@export var history_rows_node: VBoxContainer
@export var input_response_scene: PackedScene

@export var room_scene: PackedScene
@export var scroll: ScrollContainer
@export var max_lines_remembered: int = 10

@export var player: Node

const SAVE_FILE_PATH = "user://savegame.dat"

var _scroll_bar: VScrollBar
var _max_scroll_length: float = 0.0

# --- INÍCIO: Adições para Tamanho da Fonte ---
# Rastreia o tamanho da fonte atual
var _current_font_size: int = 30
const _FONT_SIZE_MIN: int = 16
const _FONT_SIZE_MAX: int = 48
const _FONT_SIZE_STEP: int = 2

# Referências para os labels que precisam ser atualizados
@onready var _caret_label: Label = $Interface/MarginContainer/HBoxContainer/Rows/HBoxContainer/InputArea/HBoxContainer/Caret
@onready var _config_title_label: RichTextLabel = $Interface/MarginContainer/HBoxContainer/VBoxContainer/Config/MarginContainer2/VBoxContainer/PanelContainer/RichTextLabel
@onready var _config_font_label: RichTextLabel = $Interface/MarginContainer/HBoxContainer/VBoxContainer/Config/MarginContainer2/VBoxContainer/PanelContainer2/VBoxContainer/HBoxContainer/RichTextLabel
# --- FIM: Adições para Tamanho da Fonte ---

func _ready() -> void:
	if input_node:
		input_node.text_submitted.connect(_on_input_submitted)
	else:
		printerr("Erro: 'input_node' não foi atribuído no GameManager.")

	if scroll:
		_scroll_bar = scroll.get_v_scroll_bar()
		_scroll_bar.changed.connect(_handle_scrollbar_changed)
		_max_scroll_length = _scroll_bar.max_value
	else:
		printerr("Erro: 'Scroll' não foi atribuído no GameManager.")

	if not player: printerr("Erro: 'Player' não foi atribuído no GameManager.")
	if not command_processor: printerr("Erro: 'CommandProcessor' não foi atribuído.")
	if not room_scene: printerr("Erro: 'Room' não foi atribuído.")
	if not input_response_scene: printerr("Erro: 'InputScene' não foi atribuído.")

	if player and command_processor:
		command_processor.set_player(player)
	else:
		printerr("Falha ao injetar Player no CommandProcessor.
O jogo não pode começar.")
		return

	_start_game()
	
	# --- INÍCIO: Adição para Tamanho da Fonte ---
	# Define o tamanho da fonte inicial para todos os elementos
	_update_all_font_sizes()
	# --- FIM: Adição para Tamanho da Fonte ---

func _start_game() -> void:
	if command_processor and room_scene:
		var starting_room = command_processor.get_starting_room_data()
		if not starting_room.is_empty():
			_add_room_node_to_game(starting_room)
		else:
			printerr("Erro: Não foi possível obter a sala inicial.")
	else:
		printerr("Erro ao iniciar o jogo: CommandProcessor ou RoomScene não estão definidos.")

func _on_input_submitted(new_text: String) -> void:
	if not input_response_scene or not \
	history_rows_node or not input_node or not command_processor or not player:
		printerr("Erro: verifique se todos os nós (Input, History, Scene, CommandProcessor, Player) estão atribuídos.")
		return

	if new_text.is_empty():
		return

	var result = command_processor.process_command(new_text)
	
	if result.type == command_processor.ResultType.META and result.command == "clear":
		_clear_history()
		input_node.text = ""
		return

	var input_response_instance = input_response_scene.instantiate()
	_add_response_to_game(input_response_instance)

	if result.type == command_processor.ResultType.ROOM:
		input_response_instance.set_text(new_text, result.message)
		_add_room_node_to_game(result.room)
		
	elif result.type == command_processor.ResultType.MESSAGE:
		input_response_instance.set_text(new_text, result.message)
		
	elif result.type == command_processor.ResultType.META:
		if result.command == "save":
			_save_game()
			input_response_instance.set_text(new_text, result.message)
		elif result.command == "load":
			var load_success = _load_game()
			if load_success:
				input_response_instance.set_text(new_text, result.message)
				var new_room_data = command_processor.get_room_data(player.localizacao)
				_add_room_node_to_game(new_room_data)
			else:
				input_response_instance.set_text(new_text, "Falha ao carregar: nenhum 
jogo salvo encontrado.")

	input_node.text = ""

func _save_game() -> void:
	if not player:
		printerr("Player não encontrado, impossível salvar.")
		return

	var save_data = {
		"nome": player.nome,
		"raca": player.raca,
		"sexo": player.sexo,
		"idade": player.idade,
		"descricao": player.descricao,
		"localizacao": player.localizacao
	}

	var file = FileAccess.open(SAVE_FILE_PATH, FileAccess.WRITE)
	if file == null:
		printerr("Erro ao tentar abrir arquivo para salvar: ", FileAccess.get_open_error())
		return

	file.store_var(save_data)
	file.close()

func _load_game() -> bool:
	if not player:
		printerr("Player não encontrado, impossível carregar.")
		return false

	if not FileAccess.file_exists(SAVE_FILE_PATH):
		return false

	var file = FileAccess.open(SAVE_FILE_PATH, FileAccess.READ)
	if file == null:
		printerr("Erro ao tentar abrir arquivo para carregar: ", FileAccess.get_open_error())
		return false

	var save_data = file.get_var()
	file.close()

	player.nome = save_data.nome
	player.raca = int(save_data.raca)
	player.sexo = save_data.sexo
	player.idade = save_data.idade
	player.descricao = save_data.descricao
	player.localizacao = save_data.localizacao
	
	return true

func _add_room_node_to_game(data) -> void:
	if not room_scene: return

	var room_node = room_scene.instantiate()

	var objects_text = "Não há objetos nesta sala."
	room_node.set_text(
		data.Nome, 
		data.Descricao, 

		data.DescricaoSaidas, 
		objects_text
	)

	_add_response_to_game(room_node)

func _add_response_to_game(response: Control) -> void:
	# --- INÍCIO: Adição para Tamanho da Fonte ---
	# Aplica o tamanho da fonte atual ao novo nó ANTES de adicioná-lo
	_apply_font_size_to_node(response, _current_font_size)
	# --- FIM: Adição para Tamanho da Fonte ---
	
	history_rows_node.add_child(response)
	_delete_input_beyond_limit()

func _delete_input_beyond_limit() -> void:
	if history_rows_node.get_child_count() > max_lines_remembered:
		var rows_to_forget = history_rows_node.get_child_count() - max_lines_remembered
		for i in range(rows_to_forget):
			history_rows_node.get_child(i).queue_free()
			
func _clear_history() -> void:
	for child in history_rows_node.get_children():
		child.queue_free()

func _handle_scrollbar_changed() -> void:
	if _max_scroll_length != _scroll_bar.max_value:
		_max_scroll_length = _scroll_bar.max_value
		scroll.scroll_vertical = int(_max_scroll_length)

# --- INÍCIO: Implementação dos Botões de Fonte ---

# Função auxiliar para aplicar o tamanho da fonte recursivamente
# aos nós RichTextLabel e Label dentro de um nó de cena (Room ou InputResponse)
func _apply_font_size_to_node(node: Control, size: int) -> void:
	# Encontra todos os RichTextLabels na cena filha
	var rich_text_labels = node.find_children("", "RichTextLabel", true, false)
	for label in rich_text_labels:
		label.set("theme_override_font_sizes/normal_font_size", size)
		label.set("theme_override_font_sizes/bold_font_size", size)
	
	# Encontra todos os Labels simples na cena filha
	var simple_labels = node.find_children("", "Label", true, false)
	for label in simple_labels:
		label.set("theme_override_font_sizes/font_size", size)

# Função auxiliar para atualizar TODOS os elementos de texto na tela
func _update_all_font_sizes() -> void:
	# 1. Atualiza o campo de input e o caret
	if input_node:
		input_node.set("theme_override_font_sizes/font_size", _current_font_size)
	if _caret_label:
		_caret_label.set("theme_override_font_sizes/font_size", _current_font_size)
	
	# 2. Atualiza os labels no painel de Configurações
	if _config_title_label:
		_config_title_label.set("theme_override_font_sizes/normal_font_size", _current_font_size)
	if _config_font_label:
		_config_font_label.set("theme_override_font_sizes/normal_font_size", _current_font_size)
	
	# 3. Atualiza todos os nós de histórico existentes
	if history_rows_node:
		for child in history_rows_node.get_children():
			_apply_font_size_to_node(child, _current_font_size)

func _on_diminuir_button_down() -> void:
	# Diminui o tamanho da fonte, respeitando o limite mínimo
	_current_font_size = max(_FONT_SIZE_MIN, _current_font_size - _FONT_SIZE_STEP)
	_update_all_font_sizes()

func _on_aumentar_button_down() -> void:
	# Aumenta o tamanho da fonte, respeitando o limite máximo
	_current_font_size = min(_FONT_SIZE_MAX, _current_font_size + _FONT_SIZE_STEP)
	_update_all_font_sizes()
# --- FIM: Implementação dos Botões de Fonte ---
