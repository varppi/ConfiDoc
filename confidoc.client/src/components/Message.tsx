import { InfoCircle } from "@untitledui/icons";

function Message({ text, color, size }: {text?: string, color?: string, size?: string}) {
  size = size == undefined ? "20" : size;
  return (
      <p className="flex gap-2 items-start whitespace-pre-line" 
        style={{ color: `var(--${color})`, fontSize: `${size}px` }}>
          <InfoCircle style={{marginTop: `${size*0.25}px`}} size={`${size}px`} /> {text.toLowerCase()}
      </p>
  );
}

export default Message;