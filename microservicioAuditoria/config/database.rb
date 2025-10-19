require 'mongo'

module Database
  def self.client
    @client ||= Mongo::Client.new('mongodb://localhost:27017/auditoria_db')
  end

  def self.eventos_collection
    client[:eventos]
  end
end
