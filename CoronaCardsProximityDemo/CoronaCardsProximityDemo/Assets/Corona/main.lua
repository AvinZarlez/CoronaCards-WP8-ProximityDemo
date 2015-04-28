-- 
-- Abstract: Hello World sample app.
--
-- Version: 1.2
-- 
-- Sample code is MIT licensed
-- Copyright (C) 2014 Corona Labs Inc. All Rights Reserved.
--
-- Supports Graphics 2.0
------------------------------------------------------------

local background = display.newImage( "world.jpg", display.contentCenterX, display.contentCenterY )

local myText = display.newText( "Hello, World!", display.contentCenterX, display.contentWidth / 4, native.systemFont, 40 )
myText:setFillColor( 1.0, 0.4, 0.4 )


-- Called when a Corona event named "messageReceived" has been dispatched
local function onMessageReceived( event )
    -- Print event property "message" to Visual Studio's Output Panel
    print( "Messaged recieved: " .. event.message )

    if (scanning) then
        local other_device = event.message

        if (other_device == player_id) then
            display_text.text = "It's a match! Winners are you"
        else
            display_text.text = "No match! You dumb"
        end
    end

    return "Done"
end



-- Dispatch an event named "startSubscribeAndPublish" to be received by C#
local startSubscribeAndPublishEvent =
{
    name = "startSubscribeAndPublish",
    message = player_id
}

local result = Runtime:dispatchEvent( startSubscribeAndPublishEvent )

-- Runtime:addEventListener( "messageReceived", onMessageReceived )
-- Runtime:removeEventListener( "messageReceived", onMessageReceived )

-- Dispatch an event named "requestingMessageBox" to be received by C#

local requestingMessageBoxEvent =
{
    name = "requestingMessageBox",
    message = "Hello World!"
}

--local result = Runtime:dispatchEvent( requestingMessageBoxEvent )

-- Print the message returned by C#
--print( tostring(result) )
