namespace ccxt.pro;

// PLEASE DO NOT EDIT THIS FILE, IT IS GENERATED AND WILL BE OVERWRITTEN:
// https://github.com/ccxt/ccxt/blob/master/CONTRIBUTING.md#how-to-contribute-code


public partial class currencycom { public currencycom(object args = null) : base(args) { } }
public partial class currencycom : ccxt.currencycom
{
    public override object describe()
    {
        return this.deepExtend(base.describe(), new Dictionary<string, object>() {
            { "has", new Dictionary<string, object>() {
                { "ws", true },
                { "watchBalance", true },
                { "watchTicker", true },
                { "watchTickers", false },
                { "watchTrades", true },
                { "watchOrderBook", true },
                { "watchOHLCV", true },
            } },
            { "urls", new Dictionary<string, object>() {
                { "api", new Dictionary<string, object>() {
                    { "ws", "wss://api-adapter.backend.currency.com/connect" },
                } },
            } },
            { "options", new Dictionary<string, object>() {
                { "tradesLimit", 1000 },
                { "OHLCVLimit", 1000 },
                { "timeframes", new Dictionary<string, object>() {
                    { "1m", "M1" },
                    { "3m", "M3" },
                    { "5m", "M5" },
                    { "15m", "M15" },
                    { "30m", "M30" },
                    { "1h", "H1" },
                    { "4h", "H4" },
                    { "1d", "D1" },
                    { "1w", "W1" },
                } },
            } },
            { "streaming", new Dictionary<string, object>() {
                { "ping", this.ping },
                { "keepAlive", 20000 },
            } },
        });
    }

    public override object ping(WebSocketClient client)
    {
        // custom ping-pong
        object requestId = ((object)this.requestId()).ToString();
        return new Dictionary<string, object>() {
            { "destination", "ping" },
            { "correlationId", requestId },
            { "payload", new Dictionary<string, object>() {} },
        };
    }

    public virtual object handlePong(WebSocketClient client, object message)
    {
        client.lastPong = this.milliseconds();
        return message;
    }

    public virtual void handleBalance(WebSocketClient client, object message, object subscription)
    {
        //
        //     {
        //         "status": "OK",
        //         "correlationId": "1",
        //         "payload": {
        //             "makerCommission": 0.2,
        //             "takerCommission": 0.2,
        //             "buyerCommission": 0.2,
        //             "sellerCommission": 0.2,
        //             "canTrade": true,
        //             "canWithdraw": true,
        //             "canDeposit": true,
        //             "updateTime": 1596742699,
        //             "balances": [
        //                 {
        //                     "accountId": 5470306579272968,
        //                     "collateralCurrency": true,
        //                     "asset": "ETH",
        //                     "free": 0,
        //                     "locked": 0,
        //                     "default": false
        //                 },
        //                 {
        //                     "accountId": 5470310874305732,
        //                     "collateralCurrency": true,
        //                     "asset": "USD",
        //                     "free": 47.82576736,
        //                     "locked": 1.187925,
        //                     "default": true
        //                 },
        //             ]
        //         }
        //     }
        //
        object payload = this.safeValue(message, "payload");
        object balance = this.parseBalance(payload);
        this.balance = this.extend(this.balance, balance);
        object messageHash = this.safeString(subscription, "messageHash");
        callDynamically(client as WebSocketClient, "resolve", new object[] {this.balance, messageHash});
        if (isTrue(inOp(((WebSocketClient)client).subscriptions, messageHash)))
        {
            ((IDictionary<string,object>)((WebSocketClient)client).subscriptions).Remove((string)messageHash);
        }
    }

    public virtual void handleTicker(WebSocketClient client, object message, object subscription)
    {
        //
        //     {
        //         "status": "OK",
        //         "correlationId": "1",
        //         "payload": {
        //             "tickers": [
        //                 {
        //                     "symbol": "BTC/USD_LEVERAGE",
        //                     "priceChange": "484.05",
        //                     "priceChangePercent": "4.14",
        //                     "weightedAvgPrice": "11682.83",
        //                     "prevClosePrice": "11197.70",
        //                     "lastPrice": "11682.80",
        //                     "lastQty": "0.25",
        //                     "bidPrice": "11682.80",
        //                     "askPrice": "11682.85",
        //                     "openPrice": "11197.70",
        //                     "highPrice": "11734.05",
        //                     "lowPrice": "11080.95",
        //                     "volume": "299.133",
        //                     "quoteVolume": "3488040.3465",
        //                     "openTime": 1596585600000,
        //                     "closeTime": 1596654452674
        //                 }
        //             ]
        //         }
        //     }
        //
        object destination = "/api/v1/ticker/24hr";
        object payload = this.safeValue(message, "payload");
        object tickers = this.safeValue(payload, "tickers", new List<object>() {});
        for (object i = 0; isLessThan(i, getArrayLength(tickers)); postFixIncrement(ref i))
        {
            object ticker = this.parseTicker(getValue(tickers, i));
            object symbol = getValue(ticker, "symbol");
            ((IDictionary<string,object>)this.tickers)[(string)symbol] = ticker;
            object messageHash = add(add(destination, ":"), symbol);
            callDynamically(client as WebSocketClient, "resolve", new object[] {ticker, messageHash});
            if (isTrue(inOp(((WebSocketClient)client).subscriptions, messageHash)))
            {
                ((IDictionary<string,object>)((WebSocketClient)client).subscriptions).Remove((string)messageHash);
            }
        }
    }

    public virtual object handleTrade(object trade, object market = null)
    {
        //
        //     {
        //         "price": 11668.55,
        //         "size": 0.001,
        //         "id": 1600300736,
        //         "ts": 1596653426822,
        //         "symbol": "BTC/USD_LEVERAGE",
        //         "orderId": "00a02503-0079-54c4-0000-00004020163c",
        //         "clientOrderId": "00a02503-0079-54c4-0000-482f0000754f",
        //         "buyer": false
        //     }
        //
        object marketId = this.safeString(trade, "symbol");
        object symbol = this.safeSymbol(marketId, null, "/");
        object timestamp = this.safeInteger(trade, "ts");
        object priceString = this.safeString(trade, "price");
        object amountString = this.safeString(trade, "size");
        object cost = this.parseNumber(Precise.stringMul(priceString, amountString));
        object price = this.parseNumber(priceString);
        object amount = this.parseNumber(amountString);
        object id = this.safeString(trade, "id");
        object orderId = this.safeString(trade, "orderId");
        object buyer = this.safeValue(trade, "buyer");
        object side = ((bool) isTrue(buyer)) ? "buy" : "sell";
        return new Dictionary<string, object>() {
            { "info", trade },
            { "timestamp", timestamp },
            { "datetime", this.iso8601(timestamp) },
            { "symbol", symbol },
            { "id", id },
            { "order", orderId },
            { "type", null },
            { "takerOrMaker", null },
            { "side", side },
            { "price", price },
            { "amount", amount },
            { "cost", cost },
            { "fee", null },
        };
    }

    public virtual void handleTrades(WebSocketClient client, object message)
    {
        //
        //     {
        //         "status": "OK",
        //         "destination": "internal.trade",
        //         "payload": {
        //             "price": 11668.55,
        //             "size": 0.001,
        //             "id": 1600300736,
        //             "ts": 1596653426822,
        //             "symbol": "BTC/USD_LEVERAGE",
        //             "orderId": "00a02503-0079-54c4-0000-00004020163c",
        //             "clientOrderId": "00a02503-0079-54c4-0000-482f0000754f",
        //             "buyer": false
        //         }
        //     }
        //
        object payload = this.safeValue(message, "payload");
        object parsed = this.handleTrade(payload);
        object symbol = getValue(parsed, "symbol");
        // const destination = this.safeString (message, 'destination');
        object destination = "trades.subscribe";
        object messageHash = add(add(destination, ":"), symbol);
        object stored = this.safeValue(this.trades, symbol);
        if (isTrue(isEqual(stored, null)))
        {
            object limit = this.safeInteger(this.options, "tradesLimit", 1000);
            stored = new ArrayCache(limit);
            ((IDictionary<string,object>)this.trades)[(string)symbol] = stored;
        }
        callDynamically(stored, "append", new object[] {parsed});
        callDynamically(client as WebSocketClient, "resolve", new object[] {stored, messageHash});
    }

    public override object findTimeframe(object timeframe, object defaultTimeframes = null)
    {
        object timeframes = this.safeValue(this.options, "timeframes", defaultTimeframes);
        object keys = new List<object>(((IDictionary<string,object>)timeframes).Keys);
        for (object i = 0; isLessThan(i, getArrayLength(keys)); postFixIncrement(ref i))
        {
            object key = getValue(keys, i);
            if (isTrue(isEqual(getValue(timeframes, key), timeframe)))
            {
                return key;
            }
        }
        return null;
    }

    public virtual void handleOHLCV(WebSocketClient client, object message)
    {
        //
        //     {
        //         "status": "OK",
        //         "destination": "ohlc.event",
        //         "payload": {
        //             "interval": "M1",
        //             "symbol": "BTC/USD_LEVERAGE",
        //             "t": 1596650940000,
        //             "h": 11670.05,
        //             "l": 11658.1,
        //             "o": 11668.55,
        //             "c": 11666.05
        //         }
        //     }
        //
        // const destination = this.safeString (message, 'destination');
        object destination = "OHLCMarketData.subscribe";
        object payload = this.safeValue(message, "payload", new Dictionary<string, object>() {});
        object interval = this.safeString(payload, "interval");
        object timeframe = this.findTimeframe(interval);
        object marketId = this.safeString(payload, "symbol");
        object market = this.safeMarket(marketId);
        object symbol = getValue(market, "symbol");
        object messageHash = add(add(add(add(destination, ":"), timeframe), ":"), symbol);
        object result = new List<object> {this.safeInteger(payload, "t"), this.safeNumber(payload, "o"), this.safeNumber(payload, "h"), this.safeNumber(payload, "l"), this.safeNumber(payload, "c"), null};
        ((IDictionary<string,object>)this.ohlcvs)[(string)symbol] = this.safeValue(this.ohlcvs, symbol, new Dictionary<string, object>() {});
        object stored = this.safeValue(getValue(this.ohlcvs, symbol), timeframe);
        if (isTrue(isEqual(stored, null)))
        {
            object limit = this.safeInteger(this.options, "OHLCVLimit", 1000);
            stored = new ArrayCacheByTimestamp(limit);
            ((IDictionary<string,object>)getValue(this.ohlcvs, symbol))[(string)timeframe] = stored;
        }
        callDynamically(stored, "append", new object[] {result});
        callDynamically(client as WebSocketClient, "resolve", new object[] {stored, messageHash});
    }

    public virtual object requestId()
    {
        object reqid = this.sum(this.safeInteger(this.options, "correlationId", 0), 1);
        ((IDictionary<string,object>)this.options)["correlationId"] = reqid;
        return reqid;
    }

    public async virtual Task<object> watchPublic(object destination, object symbol, object parameters = null)
    {
        parameters ??= new Dictionary<string, object>();
        await this.loadMarkets();
        object market = this.market(symbol);
        symbol = getValue(market, "symbol");
        object messageHash = add(add(destination, ":"), symbol);
        object url = getValue(getValue(this.urls, "api"), "ws");
        object requestId = ((object)this.requestId()).ToString();
        object request = this.deepExtend(new Dictionary<string, object>() {
            { "destination", destination },
            { "correlationId", requestId },
            { "payload", new Dictionary<string, object>() {
                { "symbols", new List<object>() {getValue(market, "id")} },
            } },
        }, parameters);
        object subscription = this.extend(request, new Dictionary<string, object>() {
            { "messageHash", messageHash },
            { "symbol", symbol },
        });
        return await this.watch(url, messageHash, request, messageHash, subscription);
    }

    public async virtual Task<object> watchPrivate(object destination, object parameters = null)
    {
        parameters ??= new Dictionary<string, object>();
        await this.loadMarkets();
        object messageHash = "/api/v1/account";
        object url = getValue(getValue(this.urls, "api"), "ws");
        object requestId = ((object)this.requestId()).ToString();
        object payload = new Dictionary<string, object>() {
            { "timestamp", this.milliseconds() },
            { "apiKey", this.apiKey },
        };
        object auth = this.urlencode(this.keysort(payload));
        object request = this.deepExtend(new Dictionary<string, object>() {
            { "destination", destination },
            { "correlationId", requestId },
            { "payload", payload },
        }, parameters);
        ((IDictionary<string,object>)getValue(request, "payload"))["signature"] = this.hmac(this.encode(auth), this.encode(this.secret), sha256);
        object subscription = this.extend(request, new Dictionary<string, object>() {
            { "messageHash", messageHash },
        });
        return await this.watch(url, messageHash, request, messageHash, subscription);
    }

    /**
     * @method
     * @name currencycom#watchBalance
     * @description watch balance and get the amount of funds available for trading or funds locked in orders
     * @param {object} [params] extra parameters specific to the exchange API endpoint
     * @returns {object} a [balance structure]{@link https://docs.ccxt.com/#/?id=balance-structure}
     */
    public async override Task<object> watchBalance(object parameters = null)
    {
        parameters ??= new Dictionary<string, object>();
        await this.loadMarkets();
        return await this.watchPrivate("/api/v1/account", parameters);
    }

    /**
     * @method
     * @name currencycom#watchTicker
     * @description watches a price ticker, a statistical calculation with the information calculated over the past 24 hours for a specific market
     * @param {string} symbol unified symbol of the market to fetch the ticker for
     * @param {object} [params] extra parameters specific to the exchange API endpoint
     * @returns {object} a [ticker structure]{@link https://docs.ccxt.com/#/?id=ticker-structure}
     */
    public async override Task<object> watchTicker(object symbol, object parameters = null)
    {
        parameters ??= new Dictionary<string, object>();
        await this.loadMarkets();
        object market = this.market(symbol);
        symbol = getValue(market, "symbol");
        object destination = "/api/v1/ticker/24hr";
        object messageHash = add(add(destination, ":"), symbol);
        object url = getValue(getValue(this.urls, "api"), "ws");
        object requestId = ((object)this.requestId()).ToString();
        object request = this.deepExtend(new Dictionary<string, object>() {
            { "destination", destination },
            { "correlationId", requestId },
            { "payload", new Dictionary<string, object>() {
                { "symbol", getValue(market, "id") },
            } },
        }, parameters);
        object subscription = this.extend(request, new Dictionary<string, object>() {
            { "messageHash", messageHash },
            { "symbol", symbol },
        });
        return await this.watch(url, messageHash, request, messageHash, subscription);
    }

    /**
     * @method
     * @name currencycom#watchTrades
     * @description get the list of most recent trades for a particular symbol
     * @param {string} symbol unified symbol of the market to fetch trades for
     * @param {int} [since] timestamp in ms of the earliest trade to fetch
     * @param {int} [limit] the maximum amount of trades to fetch
     * @param {object} [params] extra parameters specific to the exchange API endpoint
     * @returns {object[]} a list of [trade structures]{@link https://docs.ccxt.com/#/?id=public-trades}
     */
    public async override Task<object> watchTrades(object symbol, object since = null, object limit = null, object parameters = null)
    {
        parameters ??= new Dictionary<string, object>();
        await this.loadMarkets();
        symbol = this.symbol(symbol);
        object trades = await this.watchPublic("trades.subscribe", symbol, parameters);
        if (isTrue(this.newUpdates))
        {
            limit = callDynamically(trades, "getLimit", new object[] {symbol, limit});
        }
        return this.filterBySinceLimit(trades, since, limit, "timestamp", true);
    }

    /**
     * @method
     * @name currencycom#watchOrderBook
     * @description watches information on open orders with bid (buy) and ask (sell) prices, volumes and other data
     * @param {string} symbol unified symbol of the market to fetch the order book for
     * @param {int} [limit] the maximum amount of order book entries to return
     * @param {object} [params] extra parameters specific to the exchange API endpoint
     * @returns {object} A dictionary of [order book structures]{@link https://docs.ccxt.com/#/?id=order-book-structure} indexed by market symbols
     */
    public async override Task<object> watchOrderBook(object symbol, object limit = null, object parameters = null)
    {
        parameters ??= new Dictionary<string, object>();
        await this.loadMarkets();
        symbol = this.symbol(symbol);
        object orderbook = await this.watchPublic("depthMarketData.subscribe", symbol, parameters);
        return (orderbook as IOrderBook).limit();
    }

    /**
     * @method
     * @name currencycom#watchOHLCV
     * @description watches historical candlestick data containing the open, high, low, and close price, and the volume of a market
     * @param {string} symbol unified symbol of the market to fetch OHLCV data for
     * @param {string} timeframe the length of time each candle represents
     * @param {int} [since] timestamp in ms of the earliest candle to fetch
     * @param {int} [limit] the maximum amount of candles to fetch
     * @param {object} [params] extra parameters specific to the exchange API endpoint
     * @returns {int[][]} A list of candles ordered as timestamp, open, high, low, close, volume
     */
    public async override Task<object> watchOHLCV(object symbol, object timeframe = null, object since = null, object limit = null, object parameters = null)
    {
        timeframe ??= "1m";
        parameters ??= new Dictionary<string, object>();
        await this.loadMarkets();
        symbol = this.symbol(symbol);
        object destination = "OHLCMarketData.subscribe";
        object messageHash = add(add(destination, ":"), timeframe);
        object timeframes = this.safeValue(this.options, "timeframes");
        object request = new Dictionary<string, object>() {
            { "destination", destination },
            { "payload", new Dictionary<string, object>() {
                { "intervals", new List<object>() {getValue(timeframes, timeframe)} },
            } },
        };
        object ohlcv = await this.watchPublic(messageHash, symbol, this.extend(request, parameters));
        if (isTrue(this.newUpdates))
        {
            limit = callDynamically(ohlcv, "getLimit", new object[] {symbol, limit});
        }
        return this.filterBySinceLimit(ohlcv, since, limit, 0, true);
    }

    public override void handleDeltas(object bookside, object deltas)
    {
        object prices = new List<object>(((IDictionary<string,object>)deltas).Keys);
        for (object i = 0; isLessThan(i, getArrayLength(prices)); postFixIncrement(ref i))
        {
            object price = getValue(prices, i);
            object amount = getValue(deltas, price);
            (bookside as IOrderBookSide).store(parseFloat(price), parseFloat(amount));
        }
    }

    public virtual void handleOrderBook(WebSocketClient client, object message)
    {
        //
        //     {
        //         "status": "OK",
        //         "destination": "marketdepth.event",
        //         "payload": {
        //             "data": "{"ts":1596235401337,"bid":{"11366.85":0.2500,"11366.1":5.0000,"11365.6":0.5000,"11363.0":2.0000},"ofr":{"11366.9":0.2500,"11367.65":5.0000,"11368.15":0.5000}}",
        //             "symbol": "BTC/USD_LEVERAGE"
        //         }
        //     }
        //
        object payload = this.safeValue(message, "payload", new Dictionary<string, object>() {});
        object data = this.safeValue(payload, "data", new Dictionary<string, object>() {});
        object marketId = this.safeString(payload, "symbol");
        object symbol = this.safeSymbol(marketId, null, "/");
        // const destination = this.safeString (message, 'destination');
        object destination = "depthMarketData.subscribe";
        object messageHash = add(add(destination, ":"), symbol);
        object timestamp = this.safeInteger(data, "ts");
        // let orderbook = this.safeValue (this.orderbooks, symbol);
        if (!isTrue((inOp(this.orderbooks, symbol))))
        {
            ((IDictionary<string,object>)this.orderbooks)[(string)symbol] = this.orderBook();
        }
        object orderbook = getValue(this.orderbooks, symbol);
        (orderbook as IOrderBook).reset(new Dictionary<string, object>() {
            { "symbol", symbol },
            { "timestamp", timestamp },
            { "datetime", this.iso8601(timestamp) },
        });
        object bids = this.safeDict(data, "bid", new Dictionary<string, object>() {});
        object asks = this.safeDict(data, "ofr", new Dictionary<string, object>() {});
        this.handleDeltas(getValue(orderbook, "bids"), bids);
        this.handleDeltas(getValue(orderbook, "asks"), asks);
        ((IDictionary<string,object>)this.orderbooks)[(string)symbol] = orderbook;
        callDynamically(client as WebSocketClient, "resolve", new object[] {orderbook, messageHash});
    }

    public override void handleMessage(WebSocketClient client, object message)
    {
        //
        //     {
        //         "status": "OK",
        //         "correlationId": "1",
        //         "payload": {
        //             "tickers": [
        //                 {
        //                     "symbol": "1COV",
        //                     "priceChange": "-0.29",
        //                     "priceChangePercent": "-0.80",
        //                     "prevClosePrice": "36.33",
        //                     "lastPrice": "36.04",
        //                     "openPrice": "36.33",
        //                     "highPrice": "36.46",
        //                     "lowPrice": "35.88",
        //                     "openTime": 1595548800000,
        //                     "closeTime": 1595795305401
        //                 }
        //             ]
        //         }
        //     }
        //
        //     {
        //         "status": "OK",
        //         "destination": "marketdepth.event",
        //         "payload": {
        //             "data": "{"ts":1596235401337,"bid":{"11366.85":0.2500,"11366.1":5.0000,"11365.6":0.5000,"11363.0":2.0000},"ofr":{"11366.9":0.2500,"11367.65":5.0000,"11368.15":0.5000}}",
        //             "symbol": "BTC/USD_LEVERAGE"
        //         }
        //     }
        //
        //     {
        //         "status": "OK",
        //         "destination": "internal.trade",
        //         "payload": {
        //             "price": 11634.75,
        //             "size": 0.001,
        //             "id": 1605492357,
        //             "ts": 1596263802399,
        //             "instrumentId": 45076691096786110,
        //             "orderId": "00a02503-0079-54c4-0000-0000401fff51",
        //             "clientOrderId": "00a02503-0079-54c4-0000-482b00002f17",
        //             "buyer": false
        //         }
        //     }
        //
        object requestId = this.safeString(message, "correlationId");
        if (isTrue(!isEqual(requestId, null)))
        {
            object subscriptionsById = this.indexBy(((WebSocketClient)client).subscriptions, "correlationId");
            object status = this.safeString(message, "status");
            object subscription = this.safeValue(subscriptionsById, requestId);
            if (isTrue(!isEqual(subscription, null)))
            {
                if (isTrue(isEqual(status, "OK")))
                {
                    object subscriptionDestination = this.safeString(subscription, "destination");
                    if (isTrue(!isEqual(subscriptionDestination, null)))
                    {
                        object methods = new Dictionary<string, object>() {
                            { "/api/v1/ticker/24hr", this.handleTicker },
                            { "/api/v1/account", this.handleBalance },
                        };
                        object method = this.safeValue(methods, subscriptionDestination);
                        if (isTrue(isEqual(method, null)))
                        {
                            return;
                        } else
                        {
                            DynamicInvoker.InvokeMethod(method, new object[] { client, message, subscription});
                            return;
                        }
                    }
                }
            }
        }
        object destination = this.safeString(message, "destination");
        if (isTrue(!isEqual(destination, null)))
        {
            object methods = new Dictionary<string, object>() {
                { "marketdepth.event", this.handleOrderBook },
                { "internal.trade", this.handleTrades },
                { "ohlc.event", this.handleOHLCV },
                { "ping", this.handlePong },
            };
            object method = this.safeValue(methods, destination);
            if (isTrue(!isEqual(method, null)))
            {
                DynamicInvoker.InvokeMethod(method, new object[] { client, message});
            }
        }
    }
}
