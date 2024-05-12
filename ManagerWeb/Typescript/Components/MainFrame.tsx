import React, { useEffect, useState } from "react";
import Person2OutlinedIcon from '@mui/icons-material/Person2Outlined';
import NotificationsNoneOutlinedIcon from '@mui/icons-material/NotificationsNoneOutlined';
import { Badge } from "@mui/material";
import ApiClientFactory from "../Utils/ApiClientFactory";
import { useHistory } from "react-router-dom";
import { NotificationApi } from "../ApiClient/Main/apis";

const Message = ({ message, heading, isDisplayed, onClick }) => {
    return (
        <div className={`border border-vermilion bg-prussianBlue px-6 py-2 mb-2 flex flex-col rounded-md ${isDisplayed ? "" : "shadow-md shadow-vermilion"}`} onClick={onClick}>
            <h2 className="text-lg text-white">{heading}</h2>
            <p className="text-sm text-white truncate-2l">{message}</p>
        </div>
    );
}

const MainFrame = (props: { children: JSX.Element, classStyle?: string, header: string }) => {
    const [notifications, setNotifications] = useState<JSX.Element[]>([]);
    const [notDisplayedNotificationsCount, setnotDisplayedNotificationsCount] = useState<number>(0);
    const history = useHistory();
    let notificationApi: NotificationApi = undefined;

    const markNotificationAsDiplayed = async (notificationId: number) => {
        await notificationApi.notificationIdMarkAsDisplayedPut({ notificationId: notificationId });
    }

    useEffect(() => {
        const fetchData = async () => {
            try {
                const apiFactory = new ApiClientFactory(history);
                notificationApi = await apiFactory.getClient(NotificationApi);
                const notifications = await notificationApi.notificationGet();
                setnotDisplayedNotificationsCount(notifications?.filter(n => !n.isDisplayed)?.length ?? 0);
                const notifiMessages = notifications.map((n, i) => (<Message key={i} heading={n.heading} message={n.content} isDisplayed={n.isDisplayed} onClick={_ => markNotificationAsDiplayed(n.id)}></Message>))
                setNotifications(notifiMessages);
            } catch (error) {
                console.error('Error fetching data:', error);
            }
        };

        fetchData();
    }, []);

    return (
        <div>
            <div className="flex flex-row">
                <div className="w-64">
                    <svg version="1.2" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 658 403" >
                        <title>Logo3</title>
                        <defs>
                            <linearGradient id="g1" x1="54" y1="247" x2="652.1" y2="247" gradientUnits="userSpaceOnUse">
                                <stop offset=".4" stop-color="#275068" />
                                <stop offset=".4" stop-color="#275068" />
                                <stop offset="1" stop-color="#ffffff" />
                            </linearGradient>
                        </defs>
                        <style>

                        </style>
                        <path id="Tvar 3" className="s0" d="m54 336.7l39.4-34.6 56.3-16.8 14.9 17 47.6-41.3 34.9 65.3 24.3-24.6 37.2 5.9 27.6-33.6 36.4 33.8 27.1 63.7 13.9-62.1 9.3 8 17.1-45.7 7.7 3.8 29.7-62.3 10.1 32.9 5.1 14.4 14.9-8.2 38.4 7.2 36.9-41.3 28.2-68 37.4-50 3.7 292.9-598.1 0.6" />
                        <path id="B copy" className="s1" aria-label="B" d="m307.6 33.2h57.2q18.3 0 30.1 7.6 11.8 7.5 17 18.8 5.4 11.1 4 23.5-1.2 12.5-9 22.4 9.4 5.9 14.8 15.1 5.5 9.1 6.9 19.5 1.6 10.1-0.7 20.5-2.4 10.3-8.7 18.8-6.4 8.2-16.8 13.4-10.3 5.2-24.4 5.2h-70.4zm70.4 91.1h-42.2v47.3h42.2q7.5 0 12.4-3.5 5-3.6 7.3-8.7 2.6-5.2 2.6-11.3 0-6.4-2.6-11.6-2.3-5.2-7.3-8.7-4.9-3.5-12.4-3.5zm-13.2-65h-29v38.4h29.5q10.8-0.2 16.2-6.1 5.6-6.1 5.4-13.2 0-7.3-5.6-13.2-5.5-5.9-16.5-5.9z" />
                        <path id="M" className="s2" aria-label="M" d="m272.1 339l22-146.4h8.4l55.4 105.2 55.5-105.2h8.3l22 146.4h-24.7l-13-83.7-43.9 83.7h-8.4l-44.1-83.7-12.8 83.7z" />
                        <path id="I" className="s1" aria-label="I" d="m344.8 367v-146.4h25.1v146.4z" />
                        <path id="Tvar 1" className="s2" d="m46 402.4v-5h611v5z" />
                        <path id="Tvar 2" className="s2" d="m50.5 401.9h-5v-403.1h5z" />
                        <path id="$" className="s2" aria-label="$" d="m16.7 42.8v6h6.7v-6.1q2.7-0.4 4.7-2 2.1-1.7 2.9-3.8 0.8-1.7 0.8-3.5 0-3.9-2.8-6.6-3-2.8-7-2.8h-4q-1.3 0-2.2-1-0.8-1-0.8-2.2 0-1.3 0.8-2.3 0.9-1 2.2-1h9.6l3.1-6.1h-7.3v-6h-6.7v6.1q-2.8 0.4-4.8 2-2 1.7-2.9 3.8-0.7 1.7-0.7 3.5 0 1.8 0.7 3.6 1 2.3 3.4 4 2.4 1.7 5.6 1.7h4q1.3 0 2.2 1.1 0.8 1 0.8 2.2 0 1.2-0.8 2.2-0.9 1-2.2 1h-10l-3.8 6.2z" />
                        <path id="€" className="s2" aria-label="€" d="m0.2 79.6h6.3q1.7 4.4 5.8 7.5 4.1 3.1 9.7 3.1 5.3 0 9.9-3.2l0.1-0.1v-7.9l-0.8 1q-0.1 0.1-0.2 0.3-0.1 0.1-0.3 0.3-0.1 0.2-0.2 0.2-0.1 0.2-0.3 0.4-0.3 0.3-0.4 0.4-3.3 3.2-7.8 3.2-3 0-5.5-1.4-2.4-1.5-3.9-3.8h11.7l3.1-5.2h-16.4v-0.6-0.5h17l3.1-5.3h-18.5q1.5-2.3 3.9-3.7 2.5-1.5 5.5-1.5 5.4 0 8.8 4.3l0.4 0.6 3-5.1-0.2-0.1q-5-5.1-12-5.1-5.6 0-9.7 3.2-4.1 3.1-5.8 7.4h-3.2l-3.1 5.3h5.2v0.1 1h-2.1z" />
                    </svg>
                </div>
                <div className="flex flex-col pl-16 w-1/3">
                    <IconMenu Icon={Person2OutlinedIcon} countBadge={undefined} heading="Profil" messages={[]} />
                    <IconMenu Icon={NotificationsNoneOutlinedIcon} countBadge={notDisplayedNotificationsCount} heading="Notifications" messages={notifications} />
                </div>
            </div>
            <h2 className={(props.classStyle ?? "") + "text-5xl pb-2 text-center"}>{props.header}</h2>
            <div className="text-center p-2 rounded-lg">
                {props.children}
            </div>
        </div>
    );
}

export { MainFrame }

class IconMenuProps {
    Icon: React.ComponentType<any>;
    heading: string;
    countBadge: number;
    messages: Array<JSX.Element>
}

function IconMenu({ Icon, heading, countBadge, messages }: IconMenuProps) {
    const [showMenu, setShowMenu] = useState(false);

    return (
        <div className="relative">
            <div className="flex w-1/3 relative">
                <div
                    className="w-full mt-4 cursor-pointer sliding-menu-parent"
                    onClick={() => setShowMenu(!showMenu)}>
                    <div className={"absolute circle w-10 h-10 bg-white rounded-full flex items-center justify-center z-20 hover:w-full duration-500 slidingMenuSection" + (showMenu ? " slidingMenuSectionActive" : "")}>
                        <span>
                            <Icon className="fill-black" />
                        </span>
                    </div>
                    <div className="h-10 flex items-center border border-solid border-white rounded-l-full rounded-r-full z-10">
                        <span className="ml-16 pr-6">{heading}</span>
                    </div>
                </div>

                {countBadge != undefined && countBadge > 0 && (
                    <div className="absolute top-6 left-0 transform -translate-x-1/2 -translate-y-1/2 z-30">
                        <Badge badgeContent={countBadge} color="error" />
                    </div>
                )}
            </div>

            <div className={`text-black absolute top-14 mt-4 bg-white z-40 shadow-md rounded-md p-2 transition-all duration-300 ease-in-out w-2/3 ${showMenu ? 'opacity-100 h-auto' : 'opacity-0 h-0 hidden'}`} >
                {messages.map(e => (e))}
            </div>
        </div>
    );
}