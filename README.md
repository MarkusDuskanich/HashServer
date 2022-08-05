# HashServer

The goal of this project is to develop basic skills with Asp.net and the Entity framework.

The hash server can receive a message in form of a string and hash that string with sha256.
The hash will be sent back in the response. It will also be stored with the message in a database
to allow retrieval of the message with the hash.

The server will support two routes for now:

sending a message and retrieving a hash: POST api/hash

sending a hash and retrieving any messages corresponding to that hash: GET api/messages/{hash}
