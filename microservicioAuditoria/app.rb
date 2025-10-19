require 'sinatra'
require 'json'
require_relative 'models/evento'

set :port, 5003
set :bind, '0.0.0.0'

before do
  content_type :json

  if request.request_method == 'POST' && request.content_type&.include?('application/json')
    begin
      request.body.rewind
      @json_body = JSON.parse(request.body.read)
    rescue JSON::ParserError => e
      halt 400, { error: 'JSON inválido', message: e.message }.to_json
    end
  end
end

post '/auditoria' do
  begin
    halt 400, { error: 'Body JSON requerido' }.to_json unless @json_body

    evento = Evento.create(@json_body)

    status 201
    {
      message: 'Evento de auditoría creado exitosamente',
      evento: {
        id: evento['_id'].to_s,
        servicio: evento['servicio'],
        entidad: evento['entidad'],
        entidadId: evento['entidadId'],
        accion: evento['accion'],
        timestamp: evento['timestamp'],
        detalles: evento['detalles']
      }
    }.to_json
  rescue ArgumentError => e
    status 400
    { error: 'Datos inválidos', message: e.message }.to_json
  rescue => e
    status 500
    { error: 'Error interno del servidor', message: e.message }.to_json
  end
end

get '/auditoria/:entidad_id' do
  begin
    entidad_id = params[:entidad_id]
    halt 400, { error: 'entidad_id es requerido' }.to_json if entidad_id.nil? || entidad_id.strip.empty?

    eventos = Evento.find_by_entidad_id(entidad_id)

    eventos_response = eventos.map do |evento|
      {
        id: evento['_id'].to_s,
        servicio: evento['servicio'],
        entidad: evento['entidad'],
        entidadId: evento['entidadId'],
        accion: evento['accion'],
        timestamp: evento['timestamp'],
        detalles: evento['detalles']
      }
    end

    {
      message: "Eventos encontrados para entidadId: #{entidad_id}",
      total: eventos_response.length,
      eventos: eventos_response
    }.to_json
  rescue => e
    status 500
    { error: 'Error interno del servidor', message: e.message }.to_json
  end
end

get '/auditoria' do
  begin
    eventos = Evento.all

    eventos_response = eventos.map do |evento|
      {
        id: evento['_id'].to_s,
        servicio: evento['servicio'],
        entidad: evento['entidad'],
        entidadId: evento['entidadId'],
        accion: evento['accion'],
        timestamp: evento['timestamp'],
        detalles: evento['detalles']
      }
    end

    {
      message: 'Todos los eventos de auditoría',
      total: eventos_response.length,
      eventos: eventos_response
    }.to_json
  rescue => e
    status 500
    { error: 'Error interno del servidor', message: e.message }.to_json
  end
end

get '/' do
  {
    message: 'Microservicio de Auditoría',
    version: '1.0.0',
    endpoints: [
      'GET /',
      'GET /health',
      'POST /auditoria',
      'GET /auditoria',
      'GET /auditoria/:entidad_id'
    ],
    timestamp: Time.now.utc
  }.to_json
end

get '/health' do
  {
    status: 'OK',
    service: 'Microservicio de Auditoría',
    timestamp: Time.now.utc
  }.to_json
end
