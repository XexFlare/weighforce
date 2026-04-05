
 import Vue from 'vue'
  import { HubConnectionBuilder } from "@microsoft/signalr";

  import EventEmitter from 'eventemitter3';

  const defaultOptions = {
    log: false,
  };

  class SocketConnection extends EventEmitter {
    constructor(connection) {
      super();

      this.connection = connection;
      this.listened = [];
      this.socket = false;

      this.toSend = [];

      this.offline = false;
    }

    async _initialize(connection = '') {
      const con = connection || this.connection;

      try {
        const socket = new HubConnectionBuilder()
          .withUrl(con)
          .withAutomaticReconnect()
          .build()
        
        socket.connection.onclose = async (error) => {
          if (this.options.log) console.log('Reconnecting...');

          this.socket = false;
          /* eslint-disable no-underscore-dangle */
          await this._initialize(con);
          this.emit('reconnect');
        };

        await socket.start();

        this.socket = socket;
        this.emit('init');
      } catch (error) {
        if (this.options.log) console.log('Error, reconnecting...');

        setTimeout(() => {
          this._initialize(con);
        }, 1000);
      }
    }

    async start(options = {}) {
      this.options = Object.assign(defaultOptions, options);

      await this._initialize();
    }

    async authenticate(accessToken, options = {}) {
      this.connection = `${this.connection}?authorization=${accessToken}`;

      /* eslint-disable no-underscore-dangle */
      await this.start(options);
    }

    listen(method) {
      if (this.offline) return;

      if (this.listened.some(v => v === method)) return;
      this.listened.push(method);

      this.on('init', () => {
        this.socsket.on(method, (data) => {
          if (this.options.log) console.log({ type: 'receive', method, data });

          this.emit(method, data);
        });
      });
    }

    send(methodName, ...args) {
      if (this.options.log) console.log({ type: 'send', methodName, args });
      if (this.offline) return;

      if (this.socket) {
        this.socket.send(methodName, ...args);
        return;
      }

      this.once('init', () => this.socket.send(methodName, ...args));
    }

    async invoke(methodName, ...args) {
      if (this.options.log) console.log({ type: 'invoke', methodName, args });
      if (this.offline) return false;

      if (this.socket) {
        return this.socket.invoke(methodName, ...args);
      }

      return new Promise(async resolve =>
        this.once('init', () =>
          resolve(this.socket.invoke(methodName, ...args))));
    }

  }

  if (!HubConnectionBuilder) {
    throw new Error('[SigR] Cannot locate sigR-client');
  }

 export default function (Vue, connection) {
   if (!connection) {
     throw new Error('[SigR] Cannot locate connection');
   }

    const Socket = new SocketConnection(connection);

    Vue.socket = Socket;

   Object.defineProperties(Vue.prototype, {

     $socket: {
       get() {
         return {}
          return Socket;
       },
     },

   });

   Vue.mixin({

     created() {
        if (this.$options.sockets) {
          const methods = Object.getOwnPropertyNames(this.$options.sockets);

          methods.forEach((method) => {
            Socket.listen(method);

            Socket.on(method, data =>
              this.$options.sockets[method].call(this, data));
          });
        }

        if (this.$options.subscribe) {
          Socket.on('authenticated', () => {
            this.$options.subscribe.forEach((channel) => {
              Socket.invoke('join', channel);
            });
          });
        }
     },

   });
 }