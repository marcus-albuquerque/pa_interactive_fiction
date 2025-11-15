extends Node

# MESSAGE = retorna só uma mensagem
# ROOM = retorna também um objeto room
# META = um comando que o GameManager deve processar (ex: salvar)
enum ResultType { MESSAGE, ROOM, META }

@export var room_data: Resource
var _salas: Dictionary

var _player: Node = null

func _ready() -> void:
	if room_data and "salas" in room_data:
		_salas = room_data.salas
	else:
		printerr("Erro! 'room_data' não foi atribuído no CommandProcessor ou está corrompido.")
		return
	
	if _salas.is_empty():
		printerr("Erro! O dicionário de salas está vazio.")

func set_player(player_node: Node) -> void:
	_player = player_node
	if not _player:
		printerr("CommandProcessor: Referência do Player recebida é nula.")

func get_room_data(room_id: String) -> Dictionary:
	if _salas.has(room_id):
		return _salas[room_id]
	printerr("Erro: Tentativa de buscar sala inexistente: %s" % room_id)
	return {}

func get_starting_room_data() -> Dictionary:
	if not _player:
		printerr("Erro! Player não foi definido no CommandProcessor.")
		return {}
		
	return get_room_data(_player.localizacao)

func process_command(input_text: String) -> Dictionary:
	if not _player:
		return { "type": ResultType.MESSAGE, "message": "ERRO CRÍTICO: Player não definido." }
		
	var comando_limpo = input_text.strip_edges().to_lower()
	var palavras = comando_limpo.split(" ", false)

	if palavras.is_empty():
		return { "type": ResultType.MESSAGE, "message": "" }

	var primeira_palavra = palavras[0]

	match primeira_palavra:
		
		"n", "norte":
			return _mover("norte")
		"s", "sul":
			return _mover("sul")
		"l", "leste":
			return _mover("leste")
		"o", "oeste":
			return _mover("oeste")
		"c", "cima", "subir":
			return _mover("cima")
		"b", "baixo", "descer":
			return _mover("baixo")
		"v", "ver", "olhar":
			return _ver_sala()
		"l", "limpar", "clear":
			return _clear()
		"ajuda":
			return { "type": ResultType.MESSAGE, "message": _help() }
		"salvar":
			return { "type": ResultType.META, "command": "save", "message": "Jogo salvo." }
		"carregar":
			return { "type": ResultType.META, "command": "load", "message": "Jogo carregado." }
		_:
			return { "type": ResultType.MESSAGE, "message": "Comando não reconhecido." }

func _ver_sala() -> Dictionary:
	var sala_atual = _salas[_player.localizacao]
	return {
		"type": ResultType.ROOM,
		"room": sala_atual,
		"message": ""
	}

func _mover(direcao: String) -> Dictionary:
	var sala_atual = _salas[_player.localizacao]
	var saidas = sala_atual["Saidas"]

	if saidas.has(direcao):
		var proxima_sala_id = saidas[direcao]
		
		_player.localizacao = proxima_sala_id 
		
		var nova_sala = _salas[_player.localizacao]
		
		var mensagem_saida = ""
		
		match direcao:
			"cima":
				mensagem_saida = "Você sobe."
			"baixo":
				mensagem_saida = "Você desce."
			"norte", "sul", "leste", "oeste":
				mensagem_saida = "Você vai para o " + direcao + "."
			_:
				mensagem_saida = "Você vai para " + direcao + "."
		
		return {
			"type": ResultType.ROOM,
			"room": nova_sala,
			"message": mensagem_saida
		}
	else:
		return {
			"type": ResultType.MESSAGE,
			"message": "Você não pode ir nessa direção."
		}
		
func _clear() -> Dictionary:
	return {
			"type": ResultType.META,
			"command": "clear",
			"message": ""
		}

# MODIFICADO: Adiciona os novos comandos
func _help() -> String:
	return """[b]Comandos de movimento:[/b]
	- [b]norte[/b] (ou [b]n[/b])
	- [b]sul[/b] (ou [b]s[/b])
	- [b]leste[/b] (ou [b]l[/b])
	- [b]oeste[/b] (ou [b]o[/b])
	- [b]cima[/b] (ou [b]c[/b], [b]subir[/b])
	- [b]baixo[/b] (ou [b]b[/b], [b]descer[/b])
[b]Outros comandos:[/b]
	- [b]ver[/b] ([b]v[/b] ou [b]olhar[/b])
	- [b]ajuda[/b]
	- [b]salvar[/b]
	- [b]carregar[/b]"""
