function LoadContent(actor)
	local data = {}
	data.Position = Vector2(0,0)
	data.Texture = actor.Game:LoadTexture('circle')
	data.Batch = actor.Game.SpriteBatch
	data.Color = Color.Purple
	return data
end

function EditContent(data)
	data.Color = Color.Gray
end

function Update(data)
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

function Draw(data)
	data.Batch:Draw(data.Texture, data.Position, data.Color)
end

function UnloadContent(data)
end