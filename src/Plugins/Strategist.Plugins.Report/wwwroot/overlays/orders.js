const CROSS = `m512.001 84.853-84.853-84.853-171.147 171.147-171.148-171.147-84.853
        84.853 171.148 171.147-171.148 171.148 84.853 84.853 171.148-171.147
        171.147 171.147 84.853-84.853-171.148-171.148z`;

export default {
    name: 'Orders',
    mixins: [window.TradingVueLib.Overlay],
    methods: {
        meta_info() {
            return {
                author: 'businessduck',
                version: '0.0.1',
                desc: 'Trades overlay with arrows',
            };
        },
        draw(ctx) {
            ctx.strokeStyle = 'black';

            const layout = this.$props.layout;
            const candledCount = this.layout.candles.length - 1;
            const deals = [];
            const dealsMap = new Map();

            for (const deal of this.$props.data) {
                if (Array.isArray(deal[1])) {
                    const time = deal.shift();
                    deals.push(...deal.map((item) => [time, ...item]));
                } else {
                    deals.push(deal);
                }
            }

            for (const deal of deals) {
                const [time, id, type] = deal;
                const existing = dealsMap.get(id);

                if (type === 'StopLoss' && existing) {
                    existing[5] = deal[3];
                } else {
                    dealsMap.set(id, deal);
                }
            }

            dealsMap.forEach((deal) => {
                const type = deal[2];
                const orderType = deal[4];
                const isBuy = orderType === 'BUY';

                ctx.fillStyle = isBuy ? this.buy_color : this.sell_color;

                if (type === 'Both' || type === 'Close' || type === 'Reduce') {
                    const [closeTime, id, type, openPrice, openType, stopPrice, closeType, closePrice, openTime] = deal;
                    const x0 = layout.t2screen(openTime);
                    const y0 = layout.$2screen(openPrice);
                    const x1 = layout.t2screen(closeTime);
                    const y1 = layout.$2screen(closePrice);
                    const isStop = (isBuy && y0 < y1) || (!isBuy && y0 > y1);

                    this.draw_background(ctx, x0, y0, x1, y1, isStop);
                    this.draw_arrow(ctx, x0, y0, x1, y1, 1);
                } else if (type === 'Open' || type === 'Entry') {
                    const [time, id, type, price, orderType, stopPrice] = deal;
                    const lastCandle = this.layout.candles[candledCount];
                    const x0 = layout.t2screen(time);
                    const y0 = layout.$2screen(price);
                    const x1 = lastCandle.x;
                    const y1 = lastCandle.c;
                    const y2 = stopPrice && layout.$2screen(stopPrice);
                    const isStop = (isBuy && y0 < y1) || (!isBuy && y0 > y1);

                    this.draw_background(ctx, x0, y0, x1, y1, isStop);

                    if (y2) {
                        this.draw_background(ctx, x0, y0, x1, y2, true);
                    }

                    this.draw_arrow(ctx, x0, y0, x1, y1, 1);
                    this.draw_entry(ctx, x0, y0, price);
                }
            });

            // for (const deal of deals) {
            //     const [time, cid, type, price, name, closetime, closetype, closeprice, closename, stopprice] = deal;
            //     const isBuy = type === 1;
            //     const isTypeStop = closename === 'Stop';
            //     const isTypeEntry = closename === 'Entry';
            //     const isTypeOpened = closename === 'Opened';
            //     // Stop loss price
            //     const y2 = stopprice && layout.$2screen(stopprice);
            //     const nextDeal = this.$props.data[idx + 1];
            //     const closeDeal =
            //         nextDeal &&
            //         this.$props.data.find((item, index) => {
            //             if (item[8] === 'Opened' || index <= idx) {
            //                 return false;
            //             }

            //             const closeCid = item[1];

            //             return Array.isArray(closeCid) ? closeCid.includes(cid) : closeCid === cid;
            //         });

            //     let x0 = time && layout.t2screen(time);
            //     let y0 = price && layout.$2screen(price);
            //     let x1 = closetime && layout.t2screen(closetime);
            //     let y1 = closeprice && layout.$2screen(closeprice);

            //     ctx.fillStyle = isBuy ? this.buy_color : this.sell_color;

            //     if (start && (!nextDeal || nextDeal[1] !== cid)) {
            //         x0 = start[0] && layout.t2screen(start[0]);
            //         y0 = start[3] && layout.$2screen(start[3]);
            //         start = null;
            //     }

            //     if (nextDeal && nextDeal[1] === cid && !start) {
            //         start = deal;
            //         continue;
            //     }

            //     if (start && start[1] === cid) {
            //         continue;
            //     }

            //     if (isTypeOpened) {
            //         // Live order
            //         const lastCandle = this.layout.candles[candledCount];
            //         // Order loss

            //         x1 = lastCandle.x;
            //         y1 = lastCandle.c;

            //         if (closeDeal) {
            //             x1 = layout.t2screen(closeDeal[5]);
            //             y1 = layout.$2screen(closeDeal[7]);
            //         }

            //         const isStop = (isBuy && y0 < y1) || (!isBuy && y0 > x1);

            //         this.draw_background(ctx, x0, y0, x1, y1, isStop);

            //         if (y2) {
            //             this.draw_background(ctx, x0, y0, x1, y2, true);
            //         }

            //         this.draw_arrow(ctx, x0, y0, x1, y1, 1);
            //         this.draw_entry(ctx, x0, y0, deal);
            //     } else if (isTypeEntry) {
            //         if (y2) {
            //             this.draw_background(ctx, x0, y0, x1, y2, true);
            //         }

            //         this.draw_entry(ctx, x0, y0, deal);
            //     } else if (typeof cid === 'number') {
            //         this.draw_background(ctx, x0, y0, x1, y1, isTypeStop);
            //         this.draw_arrow(ctx, x0, y0, x1, y1, 1);
            //     }
            // }
        },
        draw_background(ctx, x1, y1, x2, y2, stop) {
            ctx.save();
            ctx.fillStyle = 'rgb(10, 153, 129, 0.5)';

            if (stop) {
                ctx.fillStyle = 'rgb(242, 53, 69, 0.5)';
            }

            ctx.fillRect(x1, y1, x2 - x1, y2 - y1);
            ctx.restore();
        },
        draw_entry(ctx, x, y, price) {
            ctx.save();
            ctx.beginPath();
            const startX = x - 48;
            const startY = y - 18;

            ctx.moveTo(startX, startY + 24);
            ctx.lineTo(startX + 36, startY + 24);
            ctx.lineTo(startX + 48, startY + 18);
            ctx.lineTo(startX + 36, startY + 12);
            ctx.lineTo(startX + 0, startY + 12);
            ctx.fill();
            ctx.fillStyle = 'white';
            ctx.fillText(price, startX + 6, startY + 21.5);
            ctx.restore();
        },
        draw_cross(ctx, x, y) {
            ctx.save();
            let p = new Path2D(CROSS);
            ctx.lineWidth = 150;
            ctx.translate(x - 5, y - 5);
            ctx.scale(0.0175, 0.0175);
            ctx.stroke(p);
            ctx.fill(p);
            ctx.scale(1, 1);
            ctx.restore();
        },
        draw_info(ctx, x, y, width, height, deal) {
            const radius = 3;
            ctx.save();
            ctx.lineWidth = 4;
            ctx.strokeStyle = '#000000';
            ctx.fillStyle = '#abc';
            ctx.beginPath();
            ctx.moveTo(x + radius, y);
            ctx.lineTo(x + width - radius, y);
            ctx.quadraticCurveTo(x + width, y, x + width, y + radius);
            ctx.lineTo(x + width, y + height - radius);
            ctx.quadraticCurveTo(x + width, y + height, x + width - radius, y + height);
            ctx.lineTo(x + radius, y + height);
            ctx.quadraticCurveTo(x, y + height, x, y + height - radius);
            ctx.lineTo(x, y + radius);
            ctx.quadraticCurveTo(x, y, x + radius, y);
            ctx.closePath();
            ctx.stroke();
            ctx.fill();
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';
            ctx.fillStyle = '#000000';
            const text = `Entry: ${deal[3]} / Exit: ${deal[7]}`;
            ctx.fillText(text, x + width / 2, y + height / 2);
            ctx.restore();
        },
        draw_arrow(ctx, fromx, fromy, tox, toy, arrowWidth) {
            //variables to be used when creating the arrow
            const headlen = 10;
            const angle = Math.atan2(toy - fromy, tox - fromx);

            ctx.save();
            ctx.strokeStyle = 'rgb(255, 255, 255, 0.7)';
            ctx.setLineDash([10, 5]);

            //starting path of the arrow from the start square to the end square
            //and drawing the stroke
            ctx.beginPath();
            ctx.moveTo(fromx, fromy);
            ctx.lineTo(tox, toy);
            ctx.lineWidth = arrowWidth;
            ctx.stroke();

            //starting a new path from the head of the arrow to one of the sides of
            //the point
            ctx.beginPath();
            ctx.moveTo(tox, toy);
            ctx.lineTo(tox - headlen * Math.cos(angle - Math.PI / 7), toy - headlen * Math.sin(angle - Math.PI / 7));

            //path from the side point of the arrow, to the other side point
            ctx.lineTo(tox - headlen * Math.cos(angle + Math.PI / 7), toy - headlen * Math.sin(angle + Math.PI / 7));

            ctx.setLineDash([0, 0]);
            //path from the side point back to the tip of the arrow, and then
            //again to the opposite side point
            ctx.lineTo(tox, toy);
            ctx.lineTo(tox - headlen * Math.cos(angle - Math.PI / 7), toy - headlen * Math.sin(angle - Math.PI / 7));

            //draws the paths created above
            ctx.fill();
            ctx.stroke();
            ctx.restore();
        },
        use_for() {
            return ['Orders'];
        },
        // Defines legend format (values & colors)
        legend(values) {
            let pos;

            switch (values[1]) {
                case 0:
                    pos = 'Sell';
                    break;
                case 1:
                    pos = 'Buy';
                    break;
                default:
                    pos = 'Unknown';
            }
            return [
                {
                    value: pos.toLocaleUpperCase(),
                },
                {
                    value: 'n/a',
                    // value: values[2]?.toFixed(4) || 'n/a',
                    color: this.$props.colors.colorText,
                },
                {
                    value: '→',
                },
                {
                    value: 'n/a',
                    // value: values[6]?.toFixed(4) || 'n/a',
                    color: this.$props.colors.colorText,
                },
            ];
        },
    },
    // Define internal setting & constants here
    computed: {
        sett() {
            return this.$props.settings;
        },
        default_font() {
            return '12px ' + this.$props.font.split('px').pop();
        },
        buy_color() {
            return this.sett.buyColor || 'rgb(37, 177, 247, 0.65)';
        },
        sell_color() {
            return this.sett.sellColor || 'rgb(255, 92, 92, 0.65)';
        },
        label_color() {
            return this.sett.labelColor || this.colors.text;
        },
        marker_size() {
            return this.sett.markerSize || 5;
        },
        show_label() {
            return this.sett.showLabel !== false;
        },
        new_font() {
            return this.sett.font || this.default_font;
        },
    },
};
