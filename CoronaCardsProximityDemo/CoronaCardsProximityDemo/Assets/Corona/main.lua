local background = display.newImage( "world.jpg", display.contentCenterX, display.contentCenterY )

local myText = display.newText( "Tap me!", display.contentCenterX, display.contentWidth / 4, native.systemFont, 38 )
myText:setFillColor( 1.0, 0.4, 0.4 )

-- Called when a Corona event named "messageReceived" has been dispatched
local function onMessageReceived( event )
    -- Print event property "message" to the screen
    myText.text = "It works!\n" .. event.message

    return "Done"
end

-- Dispatch an event named "startSubscribeAndPublish" to be received by C#
local startSubscribeAndPublishEvent =
{
    name = "startSubscribeAndPublish",
    message = "Hello World!"
}

local result = Runtime:dispatchEvent( startSubscribeAndPublishEvent )

Runtime:addEventListener( "messageReceived", onMessageReceived )

-- In a real app, remember to remove when you're done!
--Runtime:removeEventListener( "messageReceived", onMessageReceived )