data = {}

function LoadContent(actor)
	data.FPS = 0
	data.Counter = 0
	data.Color = Color.White
	data.Time = 0
	data.Font = actor.Game.SpriteFont
	data.Batch = actor.Game.SpriteBatch
	data.Position = Vector2(actor.Game.GraphicsDevice.Viewport.Width * 0.75,0)
end

function Update(actor, gameTime)
	data.Time = data.Time + gameTime.ElapsedGameTime.TotalSeconds

	if data.Time > 1 then
		data.FPS = data.Counter
		data.Counter = 0
		data.Time = 0
	end

end


function Draw(actor, gameTime)
	data.Counter = data.Counter + 1
	data.Batch:DrawString(data.Font, tostring(data.FPS), data.Position, data.Color)
end

function UnloadContent(actor)
end

function GetData()
	return data
end