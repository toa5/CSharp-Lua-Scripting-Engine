data = {}

function LoadContent(actor)
	data.Position = Vector2(0,0)
	data.Texture = actor.Game:LoadTexture('circle')
	data.Batch = actor.Game.SpriteBatch
	data.Color = Color.Green
end

function Update(actor, gameTime)
	state = Keyboard.GetState();

	if state:IsKeyDown(Keys.Up) then
		data.Position.Y = data.Position.Y - 10

	elseif state:IsKeyDown(Keys.Down) then
		data.Position.Y = data.Position.Y + 10
	end

	if state:IsKeyDown(Keys.Left) then
		data.Position.X = data.Position.X - 10

	elseif state:IsKeyDown(Keys.Right) then
		data.Position.X = data.Position.X + 10

	end
end


function Draw(actor, gameTime)
	data.Batch:Draw(data.Texture, data.Position, data.Color)
end

function UnloadContent(actor)
end

function GetData()
	return data
end