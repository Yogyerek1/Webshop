import { type ReactNode } from "react";

type ContainerProps = {
    bgColor: string;
    children: ReactNode;
};

export const Container = ({ children, bgColor }: ContainerProps) => {
    return (
        <div className={`min-w-screen min-h-screen ${bgColor}`}>
            {children}
        </div>
    );
};